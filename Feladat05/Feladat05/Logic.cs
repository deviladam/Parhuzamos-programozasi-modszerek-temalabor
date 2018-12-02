using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Feladat05
{
    class Logic
    {
        private const int maxWaitTime = 1;
        private const int numberOfTasks = 20;
        private const int maxNumberOfColors = 3;
        private const int taskOffset = 100000000;
        private const int maxRandomTask = int.MaxValue-100000000;
        

        private int numberOfConsumers = 4;
        private List<ColorTasks> colorTasks = new List<ColorTasks>();

        public void Init(string[] args)
        {
            
            if (args.Length > 0) { numberOfConsumers = int.Parse(args[0]); };
            for (int i = 0; i < maxNumberOfColors; i++)
            {
                colorTasks.Add(new ColorTasks());
            }
            new Thread(ProducerThread) { Name = "producer" }.Start();
            for (int i = 0; i < numberOfConsumers; i++)
            {
                new Thread(ConsumerThread) { Name = "consumer" + i }.Start();
            }
        }

        public void ProducerThread()
        {
            Console.WriteLine("ProducerThread started");
            var random = new Random();

            int waitMs = (random.Next(maxWaitTime) + 1) * 1000;
            for (int i = 0; i < numberOfTasks; i++)
            {
                Thread.Sleep(waitMs);
                int color = random.Next(maxNumberOfColors);
                colorTasks[color].Add(random.Next(maxRandomTask) + taskOffset);
                waitMs = (random.Next(maxWaitTime) + 1) * 1000;
            }
            Console.WriteLine("ProducerThread ended");
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
                            stopwatch.Restart();
                            done = FibonacciTask(n);
                            stopwatch.Stop();
                            Console.WriteLine("{0}\t{1}", stopwatch.ElapsedMilliseconds,n);
                            color.EndColorWorking();
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


        //For testing
        /*public void PrintTasks()
        {
            foreach (var item in colorTasks)
            {
                int temp;
                for (int i = 0; i < numberOfTasks; i++)
                {
                    if (item.Get(out temp))
                    {
                        Console.Write("{0}", temp);
                        item.EndColorWorking();
                    }

                }
                Console.WriteLine();
            }
        }*/
    }
}
