using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Feladat05
{
    class ColorTasks
    {
        private List<int> list = new List<int>();
        private ManualResetEvent haveTask = new ManualResetEvent(false);
        private ManualResetEvent colorNotWorking = new ManualResetEvent(true);
        private object sync = new object();

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
            if (WaitHandle.WaitAll(new WaitHandle[] {haveTask, colorNotWorking },1000))
            {
                lock (sync)
                {
                    if (list.Count > 0 )
                    {
                        task = list[0];
                        list.RemoveAt(0);
                        if (list.Count == 0)
                            haveTask.Reset();
                        colorNotWorking.Reset();
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
