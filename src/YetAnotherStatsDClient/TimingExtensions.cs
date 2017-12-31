using System;
using System.Threading.Tasks;

namespace YetAnotherStatsDClient
{
    public static class TimingExtensions
    {
        /// <summary>
        /// Start a timer for use in a "using" statement
        /// </summary>
        /// <param name="statsClient">the stats statsClient</param>
        /// <param name="bucket">the stat name</param>
        /// <returns>the disposable timer</returns>
        public static IDisposableTimer StartTimer(this IStatsClient statsClient, string bucket)
        {
            return new DisposableTimer(statsClient, bucket);
        }

        /// <summary>
        /// functional style for timing an delegate with no return value, is not async
        /// </summary>
        /// <param name="statsClient">the stats statsClient</param>
        /// <param name="bucket">the stat name</param>
        /// <param name="action">the delegate to time</param>
        public static void Time(this IStatsClient statsClient, string bucket, Action<IDisposableTimer> action)
        {
            using (var timer = StartTimer(statsClient, bucket))
            {
                action(timer);
            }
        }

        /// <summary>
        /// functional style for timing an delegate with no return value, is async
        /// </summary>
        /// <param name="statsClient">the stats statsClient</param>
        /// <param name="bucket">the stat name</param>
        /// <param name="action">the delegate to time</param>
        public static async Task Time(this IStatsClient statsClient, string bucket, Func<IDisposableTimer, Task> action)
        {
            using (var timer = StartTimer(statsClient, bucket))
            {
                await action(timer);
            }
        }

        /// <summary>
        /// functional style for timing a function with a return value, is not async
        /// </summary>
        /// <param name="statsClient">the stats statsClient</param>
        /// <param name="bucket">the stat name</param>
        /// <param name="func">the function to time</param>
        public static T Time<T>(this IStatsClient statsClient, string bucket, Func<IDisposableTimer, T> func)
        {
            using (var timer = StartTimer(statsClient, bucket))
            {
                return func(timer);
            }
        }


        /// <summary>
        /// functional style for timing a function with a return value, is async
        /// </summary>
        /// <param name="statsClient">the stats statsClient</param>
        /// <param name="bucket">the stat name</param>
        /// <param name="func">the function to time</param>
        public static async Task<T> Time<T>(this IStatsClient statsClient, string bucket, Func<IDisposableTimer, Task<T>> func)
        {
            using (var timer = StartTimer(statsClient, bucket))
            {
                return await func(timer);
            }
        }
    }
}
