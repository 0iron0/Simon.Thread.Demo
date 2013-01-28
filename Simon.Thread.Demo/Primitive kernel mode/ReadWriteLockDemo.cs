using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Simon.Threading.Demo
{
    public class ReadWriteLockDemo
    {
        private static List<string> mNames = new List<string>();
        private static System.Threading.ReaderWriterLock mLock = new System.Threading.ReaderWriterLock();

        public static void Run()
        {
            string name = "simon";

            for (int i = 0; i < 5; i++)
            {
                string newName = name + i.ToString();
                if (i < 2)
                {
                    Thread writeThread = new Thread(new ParameterizedThreadStart(Write));
                    Console.WriteLine("current request write thread:{0}", writeThread.ManagedThreadId);
                    writeThread.Start(newName);
                }
                else
                {
                    Thread readThread = new Thread(new ThreadStart(Read));
                    readThread.Start();
                }
                Thread.Sleep(10);
            }
        }

        private static void Write(object name)
        {
            if (mNames.Contains(name))
                return;
            try
            {
                mLock.AcquireWriterLock(Timeout.Infinite);//一旦获取写锁，其他写锁和读锁都将挂起
                Console.WriteLine("currrent write thread:{0}", Thread.CurrentThread.ManagedThreadId);
                mNames.Add(name.ToString());
            }
            finally
            {
                mLock.ReleaseReaderLock();
            }
        }

        private static void Read()
        {
            try
            {
                mLock.AcquireReaderLock(Timeout.Infinite);//一旦一个读锁可以访问共享区，则所有读锁都可以同时访问
                mNames.ForEach((name) => {
                    Console.WriteLine("read thread:{0}, name:{1}", Thread.CurrentThread.ManagedThreadId, name);
                });
            }
            finally
            {
                mLock.ReleaseReaderLock();
            }
        }
    }
}
