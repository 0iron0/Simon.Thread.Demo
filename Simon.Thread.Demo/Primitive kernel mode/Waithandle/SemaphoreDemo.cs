using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Simon.Threading.Demo
{
    //***********************************//
    //Semaphore(信号量)
    //信号(量)，限制同时启动线程的数量
    //***********************************//
    //Semaphore与Mutex区别
    //Mutex相当于只有一把钥匙和一把锁
    //Semaphore相当于一个club，有一定容量，满之前线程一直可以获取信号进入共享区
    //***********************************//
    public class SemaphoreDemo
    {
        private static Semaphore mSemaphore = new Semaphore(2, 2);

        public static void Run()
        {
            for (int i = 0; i < 5; i++)
            {
                string threadName = "t" + i.ToString();
                ThreadPool.QueueUserWorkItem((name) => 
                {
                    Console.WriteLine("{0} is waiting for a certificate.", name);
                    mSemaphore.WaitOne();//尝试获取信号
                    try
                    {
                        Console.WriteLine("{0} got a certificate.", name);
                        Thread.Sleep(10);
                    }
                    finally
                    {
                        Console.WriteLine("{0} released a certificate.", name);
                        mSemaphore.Release();//释放一个信号
                    }
                }, threadName);
            }

            //mSemaphore.Release();//释放所有信号
        }
    }
}
