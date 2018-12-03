using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Feladat05
{
    class Program
    {
        

        static void Main(string[] args)
        {
            Logic logic = new Logic();
            logic.Init(args);
            logic.StartProducer();
            logic.StartConsumers();



            Thread.Sleep(30000);
            logic.ThthroughPut(false);
        }


    }


}
