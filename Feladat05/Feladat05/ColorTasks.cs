using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Feladat05
{
	public class ColorTasks
	{
		private List<int> list = new List<int>();
		private ManualResetEvent haveTask = new ManualResetEvent(false);
		private AutoResetEvent colorNotWorking = new AutoResetEvent(true);
		private object sync = new object();
		private static int nextColor = -1;
		private int currColor = Interlocked.Increment(ref nextColor);

		//ColorTask.GetColor
		public int CurrColor { get => currColor; }

		public void EndColorWorking()
		{
			colorNotWorking.Set();
		}

		public void Add(int task)
		{
			lock (sync)
			{
				list.Add(task);
				haveTask.Set();
			}
		}

		public bool Get(out int task)
		{
			task = -1;
			if (haveTask.WaitOne(50) && colorNotWorking.WaitOne(50))
			{
				lock (sync)
				{
					if (list.Count > 0)
					{
						task = list[0];
						list.RemoveAt(0);
						if (list.Count == 0)
							haveTask.Reset();
						return true;
					}
					colorNotWorking.Set();
				}
			}
			return false;
		}
	}
}
