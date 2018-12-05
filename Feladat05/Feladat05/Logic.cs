using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Feladat05
{
	public class Logic
	{
		private const int maxWaitTime = 30;
		private const int taskOffset = 700000000;
		private const int maxRandomTask = int.MaxValue - 700000001;

		private int numberOfColors = 3;
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

		private int GetSolvedTasks()
		{
			lock (sync)
			{
				return solvedTasks;
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
			sw.Restart();
		}

		public void StartProducer()
		{
			new Thread(ProducerThread) { Name = "producer", IsBackground = true }.Start();
		}

		public void StartConsumers()
		{
			for (int i = 0; i < numberOfConsumers; i++)
			{
				new Thread(ConsumerThread) { Name = "consumer" + i, IsBackground = true }.Start();
			}
		}

		//kikomentezett rész logolásra szolgál 
		public void ProducerThread()
		{
			var random = new Random();
			while (true)
			{
				int color = random.Next(numberOfColors);
				//Console.WriteLine("new task\t{0}", color);
				colorTasks[color].Add(random.Next(maxRandomTask) + taskOffset);
				int waitMs = (random.Next(maxWaitTime) + 1) * 25;
				Thread.Sleep(waitMs);
			}

		}


		//kikomentezett rész logolásra szolgál 
		public void ConsumerThread()
		{
			int n = -1;
			Boolean done;
			Boolean gotOne;
			//Stopwatch stopwatch = Stopwatch.StartNew();
			while (true)
			{
				foreach (var color in colorTasks)
				{
					done = false;
					gotOne = false;
					try
					{
						if (gotOne = color.Get(out n))
						{
							//Console.WriteLine("started\t{0}", color.CurrColor);
							//stopwatch.Restart();
							done = FibonacciTask(n);
							//stopwatch.Stop();
							//Console.WriteLine("{0}\t{1}", color.CurrColor, stopwatch.ElapsedMilliseconds);
							color.EndColorWorking();
							//Console.WriteLine("ended\t{0}", color.CurrColor);
							IncSolvedTasks();
						}
					}
					finally
					{
						if (!done && gotOne)
						{
							color.Add(n);
							color.EndColorWorking();
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

			if (text)
			{
				Console.Write("ColorNumber;ConsumerNumber;ThthroughPut(Hz):");
			}
			Console.WriteLine("{0};{1};{2}", numberOfColors, numberOfConsumers, (double)GetSolvedTasks() / (sw.ElapsedMilliseconds / 1000));

		}
	}
}
