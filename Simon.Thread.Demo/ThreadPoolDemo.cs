using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Simon.Threading.Demo
{
    public class ThreadPoolDemo
    {
        private static int mCount = 5;

        public static void Run()
        {
            ManualResetEvent[] handles = new ManualResetEvent[mCount];
            Console.WriteLine("main thread id:{0}", Thread.CurrentThread.ManagedThreadId);

            for (int i = 0; i < mCount; i++)
            {
                handles[i] = new ManualResetEvent(false);
                Monitor monitor = new Monitor(handles[i]);
                ThreadPool.QueueUserWorkItem(monitor.Dispaly, new {num = 2});//parameter is a anonymous type
            }

            WaitHandle.WaitAll(handles);

            Console.WriteLine("completed.");
        }

        class Monitor
        {
            private ManualResetEvent mHandle = null;

            public Monitor(ManualResetEvent handle)
            {
                this.mHandle = handle;
            }

            public void Dispaly(object state)
            {
                Console.WriteLine("current thread id:{0}, state:{1}, value:{2}", 
                    Thread.CurrentThread.ManagedThreadId, 
                    Thread.CurrentThread.ThreadState.ToString(),
                    ((dynamic)state).num);

                //Thread.Sleep(3000);//不sleep的话，每个线程执行时间比较短，可以看到线程池会重用线程

                mHandle.Set(); //设置为绿灯
                //mHandle.Reset(); //设置为红灯
            }
        }
    }
}
