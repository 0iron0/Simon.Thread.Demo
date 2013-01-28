using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Simon.Threading.Demo
{
    public class BarrierDemo
    {
        static Barrier mBarrier = new Barrier(3, (mBarrier) => Finished());//有三个参与者
        public static void Run()
        {
            new Thread(Speak).Start();
            new Thread(Speak).Start();
            new Thread(Speak).Start();
        }

        static void Speak()
        {
            for (int i = 0; i < 5; i++)
            {
                Console.Write(i + " ");
                mBarrier.SignalAndWait(); //告知参与者已经来了，等待其他参与者都执行完该操作后，继续执行
            }
        }

        static void Finished()
        {
            Console.WriteLine("thread {0} completed.", Thread.CurrentThread.ManagedThreadId);
        }
    }
}
