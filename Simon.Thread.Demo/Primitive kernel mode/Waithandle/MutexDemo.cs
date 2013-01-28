using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Simon.Threading.Demo
{
    //***********************************//
    //Mutex(互斥)，可以用于进程间互斥
    //构造时如果不指定name，为local mutex；
    //指定name，为global mutex，可以用户进程间互斥
    //mutex name大小写敏感
    //***********************************//
    public class MutexDemo
    {
        private static char[] mTestChars = "abcdefg".ToCharArray();
        private static char[] mBackChars = new char[mTestChars.Length];

        private static System.Threading.Mutex mMutex = new System.Threading.Mutex(false, "Simon");

        public static void Run()
        {
            try
            {
                System.Threading.Mutex newMutex = System.Threading.Mutex.OpenExisting("Robin", System.Security.AccessControl.MutexRights.FullControl);
            }
            catch (WaitHandleCannotBeOpenedException)
            {
                Console.WriteLine("Robin mutex cannot be found.");
            }

            for (int i = 0; i < mTestChars.Length; i++)
            {
                System.Threading.ThreadPool.QueueUserWorkItem(new WaitCallback(ReadString));
            }

            Console.WriteLine("main thread is completed.");
        }

        private static void ReadString(object state = null)
        {
            mMutex.WaitOne();//获取锁
            try
            {
                if (mTestChars.Length > 0)
                {
                    //每次将test4的第一个字符串放入backChars,原始backChars.Length 减去被删除一个char的testChars.length
                    Array.Copy(mTestChars, 0, mBackChars, mBackChars.Length - mTestChars.Length, 1);
                    Console.WriteLine("ThreadID:{0}, current char:{1}, after chars:{2}",
                        Thread.CurrentThread.ManagedThreadId,
                        mTestChars[0],
                        string.Join(string.Empty, mBackChars));
                    //申明一个临时char数组用来存放testChars,实际作用是删除testChars的一个char
                    char[] temp = new char[mTestChars.Length - 1];
                    //将不需要删除的chars拷贝入temp
                    Array.Copy(mTestChars, 1, temp, 0, mTestChars.Length - 1);

                    mTestChars = temp;
                }
            }
            catch { }
            finally
            {
                //无论发生什么当前线程最终还是得释放互斥体的控制权
                mMutex.ReleaseMutex();
            }
        }
    }
}
