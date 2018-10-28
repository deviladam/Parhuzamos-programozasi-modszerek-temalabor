using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feladat03
{
    class Program
    {
        const int maxNumber = 1000000;

        static int size = 10;
        static int[] vec;
        static int[] copy;
        static void Main(string[] args)
        {
            if (args.Length > 0) { size = int.Parse(args[0]); };
            vec = new int[size];
            copy = new int[size];
            Stopwatch stopwatch = Stopwatch.StartNew();
            string alg = "m";
            if (args.Length > 1) alg = args[1];
            RandomArray();
            switch (alg)
            {
                case "q":
                    stopwatch.Restart();
                    QuickSort(0, size - 1);
                    stopwatch.Stop();
                    Console.WriteLine("{0}", stopwatch.ElapsedMilliseconds);
                    break;
                case "m":
                    stopwatch.Restart();
                    ParallelMerge(0, size - 1, 16);
                    stopwatch.Stop();
                    Console.WriteLine("{0}", stopwatch.ElapsedMilliseconds);
                    break;
                case "a": //mereshez
                    stopwatch.Restart();
                    QuickSort(0, size - 1);
                    stopwatch.Stop();
                    long quick = stopwatch.ElapsedMilliseconds;
                    ResetArray();
                    stopwatch.Restart();
                    ParallelMerge(0, size - 1, 16);
                    stopwatch.Stop();
                    long paral = stopwatch.ElapsedMilliseconds;
                    Console.WriteLine("{0};{1};{2}", size,quick,paral);
                    break;
                case "b": //mereshez
                    for (int i = 2; i < 1050000; i*=2)
                    {
                        ResetArray();
                        stopwatch.Restart();
                        ParallelMerge(0, size - 1, i);
                        stopwatch.Stop();
                        Console.WriteLine("{0}\t{1}", stopwatch.ElapsedMilliseconds,i);
                    }
                    break;
                default:
                    Console.WriteLine("ELső paraméter: tömb mérete Második paraméter: algoritmus q - quicksort/m - parallel merge");
                    break;
            }
        }

        static void RandomArray()
        {
            Random random = new Random();
            for (int i = 0; i < size; i++)
            {
                vec[i] = random.Next(maxNumber + 1);
                copy[i] = vec[i];
            }
        }
        static void ResetArray()
        {
            for (int i = 0; i < size; i++)
            {
                vec[i] = copy[i];
            }
        }

        static void QuickSort(int left, int right)
        {
            QuickSortInternal(left, right);
        }

        static void QuickSortInternal(int left, int right)
        {
            if (left >= right) return;

            int last = left;
            for (int k = left; k <= right - 1; k++)
            {
                if (vec[k] < vec[right])
                {
                    Swap(last, k);
                    last++;
                }
            }
            Swap(last, right);

            QuickSortInternal(left, last - 1);
            QuickSortInternal(last + 1, right);

        }

        static void Swap(int i, int j)
        {
            int temp = vec[i];
            vec[i] = vec[j];
            vec[j] = temp;
        }

        static void ParallelMerge(int l, int r, int d)
        {
            if (r - l +1< d)
            {
                QuickSort(l, r);
                return;
            }
            int m = (l + r) / 2;
            Parallel.Invoke(
                () => ParallelMerge(l, m, d),
                () => ParallelMerge(m + 1, r, d)
            );


            Merge(l, m, m + 1, r);
        }

        static void Merge(int l1, int r1, int l2, int r2)
        {
            int[] temp = new int[r2 - l1 + 1];
            int index = 0;
            int start = l1;
            while (l1 <= r1 && l2 <= r2)
            {
                if (vec[l1] <= vec[l2])
                {
                    temp[index++] = vec[l1++];
                }
                else
                {
                    temp[index++] = vec[l2++];
                }
            }
            while (l1 <= r1)
            {
                temp[index++] = vec[l1++];
            }
            while (l2 <= r2)
            {
                temp[index++] = vec[l2++];
            }
            index = 0;
            for (int i = start; i < l2; i++)
            {
                vec[i] = temp[index++];
            }
        }


        static void PrintVec()
        {
            for (int i = 0; i < size; i++)
            {
                Console.Write("{0}, ", vec[i]);
            }
            Console.WriteLine();
        }
    }
}
