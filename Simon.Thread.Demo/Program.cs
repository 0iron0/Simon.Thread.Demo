using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;

namespace Simon.Threading.Demo
{
    class Program
    {
        //***********************************//
        //同步与互斥
        //互斥是指某一资源同时只允许一个访问者对其进行访问, 具有唯一性和排它性. 但互斥无法限制访问者对资源的访问顺序, 即访问是无序的.
        //同步是在互斥的基础上(大多数情况), 通过其他机制实现访问者对资源的有序访问. 在大多数情况下, 同步已实现了互斥, 特别是所有写入资源的情况必定是互斥的.
        //***********************************//
        static void Main(string[] args)
        {
            //ThreadDemo.Run();

            //VolatileReadWriteDemo.Run();

            //VolatileDemo.Run();

            //InterlockedDemo.Run();

            //LockDemo.Run();

            //ReadWriteLockDemo.Run();

            //ReadWriteLockSlimDemo.Run();

            //MonitorDemo.Run();

            //WaitEventHandlerDemo.Run();

            //MutexDemo.Run();

            ThreadPoolDemo.Run();

            //SemaphoreDemo.Run();

            //CountdownEventDemo.Run();

            //BarrierDemo.Run();

            Console.ReadKey();
        }
    }
}
