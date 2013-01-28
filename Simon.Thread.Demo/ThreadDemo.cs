using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Simon.Threading.Demo
{
    public class ThreadDemo
    {
        public static void Run()
        {
            SuspendCase();
        }

        private static void AbortCase()
        {
            for (int i = 0; i < 5; i++)
            {
                Thread t = new Thread((num) =>
                {
                    try
                    {
                        if (((int)num) > 2)
                            Thread.CurrentThread.Abort();
                        Console.WriteLine(num.ToString());
                    }
                    catch (ThreadAbortException)
                    {
                        Console.WriteLine("current abort thread id:{0}", Thread.CurrentThread.ManagedThreadId);
                    }
                });
                t.Start(i);
                t.Join();//阻塞主线程
            }
            Console.WriteLine("completed.");
        }

        private static void SuspendCase()
        {
            Thread t = new Thread(() =>
            {
                Thread.CurrentThread.Suspend();//.net2.0开始已经废弃挂起方法
                Console.WriteLine("current abort thread id:{0}", Thread.CurrentThread.ManagedThreadId);
            });
            t.Start();

            Console.WriteLine("main thread resume child thread.");

            t.Resume();

            Console.WriteLine("completed.");
        }
    }
}
