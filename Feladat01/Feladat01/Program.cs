using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feladat01
{
	class Program
	{
		static void Main(string[] args)
		{
			const int rngMax = 1000;		//legnagyobb random szám 
			Random random = new Random();
			int size = int.Parse(args[0]);	//mátrixok mérte 
			int[,] mxA = new int[size, size];	//matrix A
			int[,] mxB = new int[size, size];    //matrix B
			int[,] mxAB = new int[size, size];	//matrix A*B
			for (int i = 0; i < size; i++)
			{
				for (int j = 0; j < size; j++)
				{
					mxA[i,j] = random.Next(rngMax+1);
					mxB[i,j] = random.Next(rngMax+1);
				}
			}
			Stopwatch stopwatch = Stopwatch.StartNew(); 
			switch (args[1])
			{
				case "\\a":
				case "\\b":
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
					PrintOut(mxAB, size);

					Console.WriteLine(stopwatch.ElapsedMilliseconds);
				//case "\\b":
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
					PrintOut(mxAB, size);
					break;
				case "\\c":
					Console.WriteLine("c");
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

		static void PrintOut(int[,] mx, int size)
		{
			for (int i = 0; i < size; i++)
			{
				for (int j = 0; j < size; j++)
				{
					Console.Write("{0}\t",mx[i,j]);
				}
				Console.WriteLine();
			}
			Console.WriteLine();
		}
	}
}
