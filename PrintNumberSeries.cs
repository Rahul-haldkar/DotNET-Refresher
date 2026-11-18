using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace HelloWorld
{
	using System;
using System.Threading;

class Program
{
    private static readonly object lockObj = new object();
    private static int number = 1;
    private const int max = 20;

    static void Main()
    {
        Thread t1 = new Thread(() => PrintNumbers(1));
        Thread t2 = new Thread(() => PrintNumbers(2));
        Thread t3 = new Thread(() => PrintNumbers(0));

        t1.Start();
        t2.Start();
        t3.Start();

        t1.Join();
        t2.Join();
        t3.Join();
    }

    static void PrintNumbers(int remainder)
    {
        while (true)
        {
            lock (lockObj)
            {
                while (number <= max && number % 3 != remainder)
                {
                    Monitor.Wait(lockObj);
                }

                if (number > max)
                {
                    Monitor.PulseAll(lockObj);
                    return;
                }

                Console.WriteLine(
                    $"Thread {Thread.CurrentThread.ManagedThreadId}: {number}");

                number++;

                Monitor.PulseAll(lockObj);
            }
        }
    }
}
}