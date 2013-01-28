using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Simon.Threading.Demo
{
    public class MonitorDemo
    {
        private static volatile int mCount = 0;//共享变量
        private static int mValue = 0;
        private static object mSyncObj = new object();

        public static void Run()
        {
            Thread writeThread1 = new Thread(new ThreadStart(Write));
            Thread writeThread2 = new Thread(new ThreadStart(Write));

            Thread readThread1 = new Thread(new ThreadStart(Read));
            Thread readThread2 = new Thread(new ThreadStart(Read));

            writeThread1.Start();
            writeThread2.Start();

            readThread1.Start();
            readThread2.Start();
            Thread.Sleep(10);
        }

        private static void Write()
        {
            try
            {
                System.Threading.Monitor.Enter(mSyncObj);
                if (mCount == 0)//当共享变量为0，当前线程进入等待区
                {
                    System.Threading.Monitor.Wait(mSyncObj);//进入等待区
                }
                ++mValue;
                Console.WriteLine("current write thread:{0}, value:{1}", Thread.CurrentThread.ManagedThreadId, mValue);
                mCount = 0;//reset共享变量
                //Thread.Sleep(1000);
            }
            finally
            {
                System.Threading.Monitor.Exit(mSyncObj);
            }
        }

        private static void Read()
        {
            try
            {
                System.Threading.Monitor.Enter(mSyncObj);
                if (mCount == 0)
                {
                    mCount = 1;//设置共享变量
                }

                Console.WriteLine("current read thread:{0}, value:{1}", Thread.CurrentThread.ManagedThreadId, mValue);
            }
            finally
            {
                System.Threading.Monitor.PulseAll(mSyncObj);//唤醒等待去全部线程进入等待区
                //System.Threading.Monitor.Pulse(mSyncObj);//唤醒一个线程进入等待去
                System.Threading.Monitor.Exit(mSyncObj);
            }
        }
    }
}
