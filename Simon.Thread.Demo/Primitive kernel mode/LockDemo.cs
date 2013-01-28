using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Simon.Threading.Demo
{
    //***********************************//
    //lock属于基元内核模式
    //基元内核模式使用windows操作系统的内核函数调度线程
    //缺点：容易导致死锁
    //注意避免使用：
    //lock(this)锁住当前对象，可能导致其他地方在调用该对象时被莫名锁住
    //lock(typeof(Type))锁住某种类型，开销比较大，一旦锁住，可能导致该类型调用都被锁住
    //lock("string")两个string变量如果都被赋值为"string"，由于string会共享内存，
    //如果锁住一个，可能导致其他string变量也被锁住
    //***********************************//
    //lock关键字实际封装了Monior.Enter()和Monitor.Exit()
    //***********************************//
    public class LockDemo
    {
        private static int mValue = 0;
        private static object mSyncObj = new object();

        public static void Run()
        {
            Console.WriteLine("main thread:{0}", Thread.CurrentThread.ManagedThreadId);
            for (int i = 0; i < 3; i++)
            {
                Thread thread = new Thread(new ThreadStart(() =>
                {
                    lock (mSyncObj)
                    {
                        int temp = 0;
                        for (int j = 0; j < 10; j++)
                        {
                            temp++;
                        }

                        mValue += temp;
                        Thread.Sleep(1000);
                        Console.WriteLine("current thread:{0}, value:{1}", Thread.CurrentThread.ManagedThreadId, mValue);
                    }
                }));
                thread.Start();
                thread.Join();
                Console.WriteLine("current thread:{0}", thread.ManagedThreadId);
            }

            Console.WriteLine(mValue);
        }
    }
}
