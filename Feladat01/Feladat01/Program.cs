using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Feladat01
{
	class Program
	{
		const int rngMax = 99;      //legnagyobb random szám 
		
		static int size = 100;      //mátrixok mérte alapból
		static int[,] mxA;          //matrix A
		static int[,] mxB;          //matrix B
		static int[,] mxAB;         //matrix A*B
		static int startHere = -1;  //Threadpoolos algoritmushoz
		static int countDown;       //Threadpoolos algoritmushoz

        static void Main(string[] args)
		{
			//inizializálás
			Random random = new Random();
			if (args.Length > 0) { size = int.Parse(args[0]); };
			mxA = new int[size, size];  
			mxB = new int[size, size];   
			mxAB = new int[size, size];
            Stopwatch stopwatch = Stopwatch.StartNew();
            string alg = "";
            if (args.Length > 1) alg = args[1];

            //A és B mátrix randomizálása
            for (int i = 0; i < size; i++)
			{
				for (int j = 0; j < size; j++)
				{
					mxA[i,j] = random.Next(rngMax+1);
					mxB[i,j] = random.Next(rngMax+1);
				}
			}

			
			//a - naiv algoritmus
            //b - naiv algoritmus (oszloponként)
            //c - Threadpoolos algoritmus
            //d - Parallel algoritmus
            //test - méréshez
			switch (alg)
			{
				case "\\a":
                    SimpleRow();
                    stopwatch.Stop();
                    ResultPrint("NaivRow", stopwatch.ElapsedMilliseconds, size);
                    break;
				case "\\b":
                    SimpleColum();
                    stopwatch.Stop();
                    ResultPrint("NaivColum", stopwatch.ElapsedMilliseconds, size);
                    break;
				case "\\c":
                    ThreadPoolAlg(args);
                    stopwatch.Stop();
                    ResultPrint("ThreadPool", stopwatch.ElapsedMilliseconds, size);
                    break;
				case "\\d":
                    ParallelAlg();
                    stopwatch.Stop();
                    ResultPrint("Parallel", stopwatch.ElapsedMilliseconds, size);
                    break;
                case "\\test": //méréshez
                        long[] times = new long[4];
                        ResetMx();
                        stopwatch.Restart();
                        SimpleRow();
                        stopwatch.Stop();
                        times[0] = stopwatch.ElapsedMilliseconds;
                        

                        ResetMx();
                        stopwatch.Restart();
                        SimpleColum();
                        stopwatch.Stop();
                        times[1] = stopwatch.ElapsedMilliseconds;
                        

                        ResetMx();
                        stopwatch.Restart();
                        ThreadPoolAlg(args);
                        stopwatch.Stop();
                        times[2] = stopwatch.ElapsedMilliseconds;
                        

                        ResetMx();
                        stopwatch.Restart();
                        ParallelAlg();
                        stopwatch.Stop();
                        times[3] = stopwatch.ElapsedMilliseconds;
                        Console.WriteLine("{0};{1};{2};{3};{4}",size, times[0], times[1], times[2], times[3]); //első  mátris mérete, többi az algoritmusok futási ideje
                    
                    
                    break;
				default:
					Console.WriteLine("Invalid parameter! 1. param: mátrix méret\t2. param: algoritmus (\\a, \\b, \\c, vagy \\d\t{3. param: max threadek száma a ThreadPoolos algoritmushoz}");
					break;
			}
        }

        //eredmény kiírás consolra (algoritmus neve;idő;mátrix mérete)
        static void ResultPrint(string alg,long ms,int size)
        {
            Console.WriteLine("{0};{1};{2}", alg, ms, size);
        }

        //Végeredmény mátrix nullázása
        static void ResetMx()
        {
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    mxAB[i, j] = 0;
                }
            }
        }

        //Egy oszlop kiszámítása (Threadpoolos algoritmushoz)
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

        // Eredmény mátrix kiírása
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

        // naiv algoritmus
        static void SimpleRow()  
        {
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    int sum = 0;
                    for (int k = 0; k < size; k++)
                    {
                        sum += mxA[i, k] * mxB[k, j];
                    }
                    mxAB[i, j] = sum;
                }
            }
        }

        //naiv algoritmus felcserélve
        static void SimpleColum() 
        {
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    int sum = 0;
                    for (int k = 0; k < size; k++)
                    {
                        sum += mxA[j, k] * mxB[k, i];
                    }
                    mxAB[j, i] = sum;
                }
            }
        }

        //ThreadPool-os algoritmus
        static void ThreadPoolAlg(string[] args) 
        {
           
            if(args.Length > 2) { 
                ThreadPool.SetMinThreads(1, 0);
                ThreadPool.SetMaxThreads(int.Parse(args[2]), 255);
            }
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
        }

        //Parallel algoritmus
        static void ParallelAlg() 
        {
            Parallel.For(0, size, delegate (int i)
            {
                for (int j = 0; j < size; j++)
                {
                    int sum = 0;
                    for (int k = 0; k < size; k++)
                    {
                        sum += mxA[i, k] * mxB[k, j];
                    }
                    mxAB[i, j] = sum;
                }

            });
        }
    }
}
