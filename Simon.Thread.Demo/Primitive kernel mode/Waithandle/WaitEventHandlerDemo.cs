using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Simon.Threading.Demo
{
    //***********************************//
    //WaitHandle：同步句柄，底层包含一个Win32的内核句柄
    //通过信号量的方式实现线程的同步(Signaled和Non-Signaled)
    //Mutex/Semaphore/EventWaitHandle都集成自WaitHandle
    //EventWaitHandle又分为AutoResetEvent和ManualResetEvent
    //***********************************//
    //EventWaitHandle相当于信号灯
    //Signaled：绿灯，WaitOne无效
    //Non-Signaled：红灯
    //***********************************//
    //AutoResetEvent和ManualResetEvent的区别：
    //ManualResetEvent.Set()可以唤起一个或多个线程
    //AutoResetEvent.Set()每次只唤醒一个
    //***********************************//
    //AutoResetEvent像一个自动的转门，Set()后门打开，允许一个线程进入，之后自动调用Reset()门自动关闭
    //ManualResetEvent像一个普通门，Set()后门打开，允许一个或者多个线程进入，直到调用Reset()关门
    //***********************************//
    //ManualResetEventSlim(.net 4.0)速度更快
    //***********************************//
    public class WaitEventHandlerDemo
    {
        private static WaitHandle[] mWaitHandles;
        private static ManualResetEvent[] mManualResetEvent;

        public static void Run()
        {
            //Case1();
            
            Case2();
        }

        private static void Case1()
        {
            List<Action> actions = new List<Action>
            {
                ()=>{Console.WriteLine("A");},
                ()=>{Console.WriteLine("B");},
                ()=>{Console.WriteLine("C");},
                ()=>{Console.WriteLine("D");},
                ()=>{Console.WriteLine("E");},
                ()=>{Console.WriteLine("F");},
                ()=>{Console.WriteLine("G");}
            };

            InvokeAllActions(actions);
        }

        private static void Case2()
        {
            mWaitHandles = new AutoResetEvent[]//定义两个信号量
            {
                new AutoResetEvent(false),
                new AutoResetEvent(false)
            };

            mManualResetEvent = new ManualResetEvent[]
            {
                new ManualResetEvent(false),
                new ManualResetEvent(false)
            };

            Dictionary<string, string> source = new Dictionary<string, string>();
            System.Threading.ThreadPool.QueueUserWorkItem(new WaitCallback(LoadSourceRes), new Resource { Collection = source, Handle = mManualResetEvent[0] });
            Dictionary<string, string> target = new Dictionary<string, string>();
            System.Threading.ThreadPool.QueueUserWorkItem(new WaitCallback(LoadTargetRes), new Resource { Collection = target, Handle = mManualResetEvent[1] });

            WaitHandle.WaitAll(mManualResetEvent);
            //int signalIndex = WaitHandle.WaitAny(mManualResetEvent, 12 * 60 * 1000);//设置超时时间，如果超时，返回超时时间；否则，返回满足等待的对象的数组下标

            Dictionary<string, string> final = MergeRes(source, target);
            foreach (string key in final.Keys)
            {
                Console.WriteLine("Key:{0}, Value:{1}", key, final[key]);
            }

            foreach (WaitHandle handle in mManualResetEvent)
            {
                (handle as ManualResetEvent).Reset();//设置信号为红灯，对于AutoResetEvent来说，调用Set()会自动调用Reset()，无需手动调用
            }
        }

        private static void LoadSourceRes(object source)
        {
            Resource resource = source as Resource;
            //(resource.Handle as AutoResetEvent).WaitOne();由于初始化AutoResetEvent是false，即红灯，调用WaitOne()将阻塞当前线程
            try
            {
                resource.Collection.Add("a", "a");
                resource.Collection.Add("b", "b");
                Console.WriteLine("source thread: {0}", Thread.CurrentThread.ManagedThreadId);
            }
            finally
            {
                (resource.Handle as ManualResetEvent).Set();
            }
        }

        private static void LoadTargetRes(object target)
        {
            Resource resource = target as Resource;
            try
            {
                resource.Collection.Add("a", "1");
                resource.Collection.Add("b", "2");
                Console.WriteLine("target thread: {0}", Thread.CurrentThread.ManagedThreadId);
            }
            finally
            {
                (resource.Handle as ManualResetEvent).Set();//设置当前信号为绿灯
            }

        }

        private static Dictionary<string, string> MergeRes(Dictionary<string, string> source, Dictionary<string, string> target)
        {
            Dictionary<string, string> final = new Dictionary<string, string>();
            foreach (string key in source.Keys)
            {
                if (target.ContainsKey(key))
                {
                    final[key] = target[key];
                }
            }
            return final;
        }

        private static void InvokeAllActions(List<Action> actions)
        {
            mWaitHandles = new WaitHandle[actions.Count];

            if (actions == null || actions.Count == 0)
                return;

            int i = 0;
            actions.ForEach((action) =>
            {
                mWaitHandles[i] = new AutoResetEvent(false);

                System.Threading.ThreadPool.QueueUserWorkItem(new WaitCallback((handle) =>
                {
                    action.Invoke();

                    Thread.Sleep(new Random().Next(100, 1000));

                    (handle as AutoResetEvent).Set();
                }), mWaitHandles[i]);

                if (i == actions.Count - 1)
                    return;
                i++;
            });

            WaitHandle.WaitAll(mWaitHandles);//阻塞主线程，等待所有子线程发送信号，才能继续执行

            Console.WriteLine("completed.");
        }

        class Resource
        {
            public Dictionary<string, string> Collection { get; set; }
            public WaitHandle Handle { get; set; }
        }
    }
}
