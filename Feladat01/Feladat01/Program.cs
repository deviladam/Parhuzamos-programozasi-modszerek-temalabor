using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Feladat01
{
	class Program
	{
		const int rngMax = 99;        //legnagyobb random szám 
		
		static int size = 1000; //mátrixok mérte 
		static int[,] mxA;          //matrix A
		static int[,] mxB;         //matrix B
		static int[,] mxAB;     //matrix A*B
		static int startHere = -1;
		static int countDown;

		static void Main(string[] args)
		{
			
			Random random = new Random();
			if (args.Length > 0) { size = int.Parse(args[0]); };
			mxA = new int[size, size];  
			mxB = new int[size, size];   
			mxAB = new int[size, size];  

			for (int i = 0; i < size; i++)
			{
				for (int j = 0; j < size; j++)
				{
					mxA[i,j] = random.Next(rngMax+1);
					mxB[i,j] = random.Next(rngMax+1);
				}
			}
			Stopwatch stopwatch = Stopwatch.StartNew();
			string alg = "\\c";
			if (args.Length > 1) alg = args[1];
			switch (alg)
			{
				case "\\a":
					for (int i = 0; i < size; i++)
					{
						for (int j = 0; j < size; j++)
						{
							int sum = 0;
							for (int k = 0; k < size; k++)
							{
								sum += mxA[i, k] * mxB[k, j];
							}
							mxAB[i,j] = sum;
						}
					}
					break;
				case "\\b":
					for (int i = 0; i < size; i++)
					{
						for (int j = 0; j < size; j++)
						{
							int sum = 0;
							for (int k = 0; k < size; k++)
							{
								sum += mxA[j, k] * mxB[k, i];
							}
							mxAB[j,i] = sum;
						}
					}
					break;
				case "\\c":
					countDown = size;
					using (ManualResetEvent manualResetEvent = new ManualResetEvent(false))
					{
						startHere = -1;
						for (int j = 0; j < size; j++)
						{
							ThreadPool.QueueUserWorkItem(ColumnSum, manualResetEvent);
						}
						manualResetEvent.WaitOne();
					}
					break;
				case "\\d":
					Console.WriteLine("d");
					break;
				default:
					Console.WriteLine("Invalid parameter! 1. param: mátrix méret\t2. param: algoritmus (\\a, \\b, \\c, vagy \\d)");
					break;
			}
			stopwatch.Stop();
			Console.WriteLine(stopwatch.ElapsedMilliseconds);
		}

		static void ColumnSum(object arg)
		{
			startHere++;
			int j = startHere;
			
			for (int i = 0; i < size; i++)
			{
				int sum = 0;
				for (int k = 0; k < size; k++)
				{
					sum += mxA[i, k] * mxB[k, j];
				}
				mxAB[i, j] = sum;
			}
			ManualResetEvent mrEvent = (ManualResetEvent)arg;
			if (Interlocked.Decrement(ref countDown) == 0)
				mrEvent.Set();
		}

		static void PrintOut()
		{
			for (int i = 0; i < size; i++)
			{
				for (int j = 0; j < size; j++)
				{
					Console.Write("{0}\t",mxAB[i,j]);
				}
				Console.WriteLine();
			}
			Console.WriteLine();
		}
	}
}
