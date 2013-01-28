using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Simon.Threading.Demo
{
    public class CountdownEventDemo
    {
        private static CountdownEvent mCount = new CountdownEvent(3);//初始化3个信号

        public static void Run()
        {
            for (int i = 0; i < 3; i++)
            {
                ThreadPool.QueueUserWorkItem((num) =>
                {
                    Console.WriteLine(num.ToString());
                    mCount.Signal();//释放一个信号
                }, i);
            }
            mCount.Wait();//阻塞主线程，直到所有信号被释放，继续执行主线程

            Console.WriteLine("completed");
        }
    }
}
