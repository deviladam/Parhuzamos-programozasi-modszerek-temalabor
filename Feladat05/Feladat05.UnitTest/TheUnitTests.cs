using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Feladat05.UnitTest
{
    [TestClass]
    public class TheUnitTests
    {
        [TestMethod]
        public void StartProducer_GenerateMoreTask()
        {
            var args = new string[]{"2", "1"};
            var logic = new Logic();
            int temp;
            logic.Init(args);

            var result = logic.colorTasks[0].Get(out temp);

            Assert.IsFalse(result);

            logic.StartProducer();
            Thread.Sleep(1250);//wait 

            result = logic.colorTasks[0].Get(out temp);

            Assert.IsTrue(result);

        }

        [TestMethod]
        public void CouldNotRunSameColorAtTheSameTime()
        {
            var args = new string[] { "2", "1" };
            var logic = new Logic();
            int temp;

            logic.Init(args);
            logic.colorTasks[0].Add(int.MaxValue);
            var result1 = logic.colorTasks[0].Get(out temp);
            var result2 = logic.colorTasks[0].Get(out temp);

            Assert.IsTrue(result1);
            Assert.IsFalse(result2);

        }

        [TestMethod]
        public void RunColorsCorrectWay_FirstInFirstOut()
        {
            var args = new string[] { "2", "1" };
            var logic = new Logic();
            int temp;

            logic.Init(args);
            logic.colorTasks[0].Add(123);
            logic.colorTasks[0].Add(345);

            var result1 = logic.colorTasks[0].Get(out temp);
            Assert.AreEqual(123, temp);
            logic.colorTasks[0].EndColorWorking();
            var result2 = logic.colorTasks[0].Get(out temp);
            Assert.AreEqual(345, temp);
        }

        [TestMethod]
        public void WhenThreadFault_TaskGoesBack()
        {
            var args = new string[] { "2", "1" };
            var logic = new Logic();
            int temp;

            logic.Init(args);
            logic.colorTasks[0].Add(int.MaxValue);
            Thread consumer = new Thread(logic.ConsumerThread);
            consumer.Start();
            Thread.Sleep(100);
            consumer.Abort();

            var result = logic.colorTasks[0].Get(out temp);
            Assert.IsTrue(result);
            Assert.AreEqual(int.MaxValue, temp);
        }

        [TestMethod]
        public void TwoDifferentColor_CanRunAtTheSameTime()
        {
            var args = new string[] { "2", "2" };
            var logic = new Logic();
            int temp;

            logic.Init(args);
            logic.colorTasks[0].Add(int.MaxValue);
            logic.colorTasks[1].Add(int.MaxValue);
            Thread consumer = new Thread(logic.ConsumerThread);
            consumer.Start();
            Thread consumer2 = new Thread(logic.ConsumerThread);
            consumer2.Start();

            var result1 = logic.colorTasks[0].Get(out temp);
            var result2 = logic.colorTasks[0].Get(out temp);
            Assert.IsFalse(result1);
            Assert.IsFalse(result2);
        }
    }
}
