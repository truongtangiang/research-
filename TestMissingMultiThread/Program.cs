using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMissingMultiThread
{
    class Program
    {
        static object flag = new object();
        static int count = 0;
        static Random rand = new Random();
        static Stack<int> stack = new Stack<int>();
        static List<int> resultList = new List<int>();
        static List<int> rawList = new List<int>();
        static Dictionary<string, int> resultDic = new Dictionary<string, int>();

        static void Main(string[] args)
        {
            var sw = new StreamWriter("log.txt", true);


            for (int i = 0; i < 5; i++)
            {
                TestThread(5, 500);
                Console.WriteLine("Count {0}", count);
                Console.WriteLine("Miss: Raw list vs result dic: {0}", string.Join(",", rawList.Except(resultDic.Values.ToList()).ToArray()));
                Console.WriteLine("Miss: Raw list vs resultList dic: {0}", string.Join(",", rawList.Except(resultList).ToArray()));

                sw.WriteLine("Count {0}", count);
                sw.WriteLine("Miss: Raw list vs result dic: {0}", string.Join(",", rawList.Except(resultDic.Values.ToList()).ToArray()));
                sw.WriteLine("Miss: Raw list vs resultList dic: {0}", string.Join(",", rawList.Except(resultList).ToArray()));
            }

            sw.Close();
            Console.Read();
        }


        static void TestThread(int totalThread, int range)
        {
            for (int i = 0; i < range; i++)
            {
                rawList.Add(i);
                stack.Push(i);
            }

            Task[] tasks = new Task[totalThread];

            for (int i = 0; i < totalThread; i++)
            {
                tasks[i] = Task.Factory.StartNew(ProcessData);
            }

            Task.WaitAll(tasks);

            Console.WriteLine(resultList.Count);
        }
        static void ProcessData()
        {
            while (true)
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                int data = getStack();
                var delay = rand.Next(200, 2000);
                if (data == -1)
                {
                    break;
                }
                else
                {

                    Task.Delay(delay).Wait();
                    resultDic.Add(data.ToString(), data);
                    resultList.Add(data);
                    sw.Stop();
                    Console.WriteLine("Complete {0}, {1}s", data, Math.Round(sw.ElapsedMilliseconds / 1000.0, 3));

                }

            }
        }

        static int getStack()
        {
            lock (flag)
            {
                if (stack.Count > 0)
                {
                    count++;
                    return stack.Pop();
                }
                else
                {
                    return -1;
                }
            }

        }
    }

}
