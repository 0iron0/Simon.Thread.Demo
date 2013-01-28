using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Simon.Threading.Demo
{
    public class InterlockedDemo
    {
        private static long mCount = 0;//共享变量

        public static void Run()
        {
            for (int i = 0; i < 3; i++)
            {
                Thread addThread = new Thread(new ThreadStart(Add));
                Thread removeThread = new Thread(new ThreadStart(Remove));

                addThread.Start();
                Thread.Sleep(10);//阻塞主线程
                removeThread.Start();
                Thread.Sleep(10);
            }

            System.Threading.Interlocked.Add(ref mCount, 2);
            Console.WriteLine("after adding value:{0}", mCount);

            System.Threading.Interlocked.CompareExchange(ref mCount, 1, 2);
            Console.WriteLine("after exchange value:{0}", mCount);
        }

        private static void Add()
        {
            if(System.Threading.Interlocked.Read(ref mCount) == 0)
            {
                //mCount++;//it is not an atomic operation
                Console.WriteLine("Thread ID:{0} enter share area 1.", Thread.CurrentThread.ManagedThreadId);

                System.Threading.Interlocked.Increment(ref mCount);
                //mCount++;
            }
            Console.WriteLine(mCount);
        }

        private static void Remove()
        {
            if(System.Threading.Interlocked.Read(ref mCount) == 1)
            {
                //mCount--;
                Console.WriteLine("Thread ID:{0} enter share area 1.", Thread.CurrentThread.ManagedThreadId);

                System.Threading.Interlocked.Decrement(ref mCount);
                //mCount--;
            }
            Console.WriteLine(mCount);
        }
    }
}
