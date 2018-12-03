using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Feladat05
{
    public class Logic
    {
        private const int maxWaitTime = 30;
        private const int taskOffset = 100000000;
        private const int maxRandomTask = 800000000;

        private int numberOfColors = 2;
        private int numberOfConsumers = 10;
        public List<ColorTasks> colorTasks = new List<ColorTasks>();

        private static int solvedTasks = 0;
        private object sync = new object();
        private static Stopwatch sw = Stopwatch.StartNew();

        private void IncSolvedTasks()
        {
            lock (sync)
            {
                solvedTasks++;
            }
        }

        private void ResetSolvedTasks()
        {
            lock (sync)
            {
                solvedTasks = 0;
            }
        }

        public void Init(string[] args)
        {

            if (args.Length > 0) { numberOfConsumers = int.Parse(args[0]); };
            if (args.Length > 1) { numberOfColors = int.Parse(args[1]); };

            for (int i = 0; i < numberOfColors; i++)
            {
                colorTasks.Add(new ColorTasks());
            }            
        }

        public void StartProducer()
        {
            new Thread(ProducerThread) { Name = "producer", IsBackground = true}.Start();
            sw.Restart();
        }

        public void StartConsumers()
        {
            for (int i = 0; i < numberOfConsumers; i++)
            {
                new Thread(ConsumerThread) { Name = "consumer" + i, IsBackground = true }.Start();
            }
        }

        public void ProducerThread()
        {
            var random = new Random();
            while(true)
            {
                int color = random.Next(numberOfColors);
                //Console.WriteLine("new task\t{0}", color);
                colorTasks[color].Add(random.Next(maxRandomTask) + taskOffset);
                int waitMs = (random.Next(maxWaitTime) + 1) * 25;
                Thread.Sleep(waitMs);
            }
            
        }

        public void ConsumerThread()
        {
            int n;
            Boolean done;
            Stopwatch stopwatch = Stopwatch.StartNew();
            while (true)
            {
                foreach (var color in colorTasks)
                {
                    done = false;
                    if (color.Get(out n))
                    {
                        try
                        {
                            //Console.WriteLine("started\t{0}",color.GetColor());
                            done = FibonacciTask(n);
                            color.EndColorWorking();
                            //Console.WriteLine("ended\t{0}", color.GetColor());
                            IncSolvedTasks();
                        }
                        finally
                        {
                            if (!done)
                            {
                                color.Add(n);
                                color.EndColorWorking();
                            }
                        }
                    }
                }
            }
        }

        private bool FibonacciTask(int n)
        {
            int a = 0;
            int b = 1;
            for (int i = 0; i < n; i++)
            {
                int temp = a;
                a = b;
                b = a + temp;
            }
            if (a == -2)
            {
                Console.WriteLine(a);
            }
            return true;
        }

        public void ThthroughPut(Boolean text)
        {
            sw.Stop();
            if (text)
            {
                Console.Write("ColorNumber;ConsumerNumber;ThthroughPut(Hz):");
            }
            Console.WriteLine("{0};{1};{2}",numberOfColors,numberOfConsumers, (double) solvedTasks / (sw.ElapsedMilliseconds/1000));
            sw.Restart();
            ResetSolvedTasks();
        }
    }
}
