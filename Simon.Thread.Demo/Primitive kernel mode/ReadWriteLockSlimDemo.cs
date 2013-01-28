using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Simon.Threading.Demo
{
    public class ReadWriteLockSlimDemo
    {
        static ReaderWriterLockSlim mReadWriteLock = new ReaderWriterLockSlim();
        static List<int> mData = new List<int>();

        public static void Run()
        {
            //两个写线程
            new Thread(Write).Start();
            new Thread(Write).Start();

            ///三个读线程
            new Thread(Read).Start();
            new Thread(Read).Start();
            new Thread(Read).Start();

            Console.WriteLine("main thread completed.");
        }

        static void Write()
        {
            mReadWriteLock.EnterWriteLock();
            try
            {
                int item = new Random().Next(10, 1000);
                mData.Add(item);
                Console.WriteLine("write thread. item:{0}", item);
                Thread.Sleep(500);
            }
            finally
            {
                mReadWriteLock.ExitWriteLock();
            }
        }

        static void Read()
        {
            //while (true)
            //{
                mReadWriteLock.EnterReadLock();
                try
                {
                    foreach (int item in mData)
                    {
                        Console.WriteLine("read thread id:{0}, item:{1}", 
                            Thread.CurrentThread.ManagedThreadId, 
                            item.ToString());
                        Thread.Sleep(100);
                    }
                }
                finally
                {
                    mReadWriteLock.ExitReadLock();
                }
            //}
        }
    }
}
