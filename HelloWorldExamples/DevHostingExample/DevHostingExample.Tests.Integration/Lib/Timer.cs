using System;
using System.Diagnostics;

namespace DevHostingExample.Tests.Integration
{
    internal static class Timer
    {
        public static void Time(string actionDescription, Action toRun)
        {
            var stopwatch = new Stopwatch();

            stopwatch.Start();

            toRun();

            stopwatch.Stop();

            Console.WriteLine("{0}: took {1:F0}ms",
                                actionDescription,
                                stopwatch.ElapsedMilliseconds);
        }
    }
}
