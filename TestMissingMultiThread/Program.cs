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
        static object _flag = new object();
        static int _count = 0;
        static Random rand = new Random();
        static Stack<int> _stack;
        static List<int> _resultList;
        static List<int> _rawList;
        static Dictionary<string, int> _resultDic;

        static void Main(string[] args)
        {
            var sw = new StreamWriter("log.txt", true);


            for (int i = 0; i < 5; i++)
            {
                _stack = new Stack<int>();
                _resultList = new List<int>();
                _rawList = new List<int>();
                _resultDic = new Dictionary<string, int>();
                sw.WriteLine("\n"+DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"));
                TestThread(5, 500);
                Console.WriteLine("Count {0}", _count);
                Console.WriteLine("Miss: Raw list vs result dic: {0}", string.Join(",", _rawList.Except(_resultDic.Values.ToList()).ToArray()));
                Console.WriteLine("Miss: Raw list vs resultList dic: {0}", string.Join(",", _rawList.Except(_resultList).ToArray()));

                sw.WriteLine("Count {0}", _count);
                sw.WriteLine("Miss: Raw list vs result dic: {0}", string.Join(",", _rawList.Except(_resultDic.Values.ToList()).ToArray()));
                sw.WriteLine("Miss: Raw list vs resultList dic: {0}", string.Join(",", _rawList.Except(_resultList).ToArray()));
            }

            sw.Close();
            Console.Read();
        }


        static void TestThread(int totalThread, int range)
        {
            for (int i = 0; i < range; i++)
            {
                _rawList.Add(i);
                _stack.Push(i);
            }

            Task[] tasks = new Task[totalThread];

            for (int i = 0; i < totalThread; i++)
            {
                tasks[i] = Task.Factory.StartNew(ProcessData);
            }

            Task.WaitAll(tasks);

            Console.WriteLine(_resultList.Count);
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
                    _resultDic.Add(data.ToString(), data);
                    _resultList.Add(data);
                    sw.Stop();
                    Console.WriteLine("Complete {0}, {1}s", data, Math.Round(sw.ElapsedMilliseconds / 1000.0, 3));

                }

            }
        }

        static int getStack()
        {
            lock (_flag)
            {
                if (_stack.Count > 0)
                {
                    _count++;
                    return _stack.Pop();
                }
                else
                {
                    return -1;
                }
            }

        }
    }

}
