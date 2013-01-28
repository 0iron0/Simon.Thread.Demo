using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Simon.Threading.Demo
{
    #region Thread Summary
    //***********************************//
    //多线程分为基元(Primitive)用户模式和内核模式
    //基元用户模式本质是通过CPU指令调度线程
    //优点：速度快/线程阻塞时间特别短
    //缺点：由于线程被系统抢占(同时共享资源也被抢占)，
    //同时另一个线程使用用户模式也要获取该共享资源，
    //导致浪费CPU资源，这种情况又称“活锁”
    //***********************************//
    //临界区（共享区）：多个线程共同使用的资源
    //比如：变量，方法
    //这些共享的区域称为临界区
    //***********************************//
    //原子(Atomic)操作
    //不能再被分解的最小操作，比如赋值操作
    //增加变量操作(i++)不是一个原子操作
    ///过程：
    //1.将实例变量中的值加载到寄存器中。
    //2.增加或减少该值。
    //3.在实例变量中存储该值。
    //多线程环境下，线程有可能在执行了前两个操作后，被其他线程抢占，这样就会影响操作的结果
    //可以使用Interlocked实现线程同步
    //***********************************//
    //.net提供的基元用户模式下同步变量的类
    //方法：VolatileRead/VolatileWrite/Volatile/Interlocked
    //***********************************//
    //VolatileWrite本质就是赋值操作
    //由于编译器优化可能将赋值操作提前(改变顺序)，底层使用Thread.MemoryBarrior(内存栅栏)
    //避免编译器优化
    //VolatileRead使用Thread.MemoryBarrior避免编译器优化，并且避免读取缓存的值，保证变量读取的是最新的值
    //***********************************//
    //Volatile keyword是对VolatileRead和VolatileWrite的简化
    //***********************************// 
    //Conclusion:
    //基元用户模式通过CPU指令实现多线程对同一变量操作的同步，上层看相当于非阻塞同步
    //在复杂环境下，有可能出现“活锁”的情况(浪费CPU和内存)
    //***********************************// 
    #endregion
    public class VolatileReadWriteDemo
    {
        private static Int32 mCount;//共享变量
        private static Int32 mValue;

        public static void Run()
        {
            Thread readThread = new Thread(new ThreadStart(Read));
            readThread.IsBackground = true;
            readThread.Start();

            for (int i = 0; i < 10; i++)
            {
                Thread writeThread = new Thread(new ThreadStart(Write));
                writeThread.Start();
                writeThread.Join();
            }

            Console.WriteLine("main thread is completed.");
        }

        private static void Write()
        {
            int temp = 0;
            for (int i = 0; i < 1000; i++)
            {
                temp += 1;
            }
            mValue += temp;
            Console.WriteLine("Write thread ID:{0}, value:{1}", Thread.CurrentThread.ManagedThreadId, mValue);
            Thread.VolatileWrite(ref mCount, 1);//原子操作的互斥
            Thread.Sleep(100);
        }

        private static void Read()
        {
            while (true)
            {
                if (Thread.VolatileRead(ref mCount) == 1)
                {
                    Console.WriteLine("Read thread ID:{0}, total:{1}", Thread.CurrentThread.ManagedThreadId, mValue);
                    mCount = 0;
                }
            }
        }
    }

    public class VolatileDemo
    {
        private static volatile Int32 mCount;//共享区
        private static Int32 mValue;

        public static void Run()
        {
            Thread readThread = new Thread(new ThreadStart(Read));
            readThread.Start();

            for (int i = 0; i < 10; i++)
            {
                Thread writeThread = new Thread(new ThreadStart(Write));
                writeThread.Start();
            }
        }

        private static void Write()
        {
            int temp = 0;
            for (int i = 0; i < 10000000; i++)
            {
                temp += 1;
            }
            mValue += temp;
            Console.WriteLine("Write thread ID:{0}, total:{1}", Thread.CurrentThread.ManagedThreadId, mValue);
            mCount = 1;
        }

        private static void Read()
        {
            while (true)
            {
                if (mCount == 1)
                {
                    Console.WriteLine("Read thread ID:{0}, total:{1}", Thread.CurrentThread.ManagedThreadId, mValue);
                    mCount = 0;//reset
                }
            }
        }
    }
}
