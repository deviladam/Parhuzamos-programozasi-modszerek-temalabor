using System;
using System.Collections.Generic;
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
					mxA[i,j] = random.Next(rngMax);
					mxB[i,j] = random.Next(rngMax);
				}
			}
		}
	}
}
