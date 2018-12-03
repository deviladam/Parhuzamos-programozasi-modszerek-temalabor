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
        private ManualResetEvent colorNotWorking = new ManualResetEvent(true);
        private object syncPut = new object();
        private object syncGet = new object();
        private static int nextColor = 0;
        private int currColor = nextColor++;

        public void EndColorWorking()
        {
            lock (syncGet)
            {
                colorNotWorking.Set();
            }
        }

        public void Add(int task)
        {
            lock (syncPut)
            {
                list.Add(task);
                haveTask.Set();
            }
        }

        public bool Get(out int task)
        {
            task = -1;
            lock (syncGet)
            { 
                if (WaitHandle.WaitAll(new WaitHandle[] { colorNotWorking },50))
                {
                    if (WaitHandle.WaitAll(new WaitHandle[] {haveTask  }, 50)) { 
                        lock (syncPut)
                        {
                            if (list.Count > 0 )
                            {
                                colorNotWorking.Reset();
                                task = list[0];
                                list.RemoveAt(0);
                                if (list.Count == 0)
                                    haveTask.Reset();
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        public int GetColor()
        {
            return currColor;
        }
    }
}
