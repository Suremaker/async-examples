using System;
using System.Threading;
using System.Threading.Tasks;
using Examples.Helpers;

namespace Examples
{
    class Methods
    {
        public static void ThreadSleep(int secs)
        {
            Logger.Log($"sleeping {secs}s");

            Thread.Sleep(TimeSpan.FromSeconds(secs));

            Logger.Log($"finished sleeping {secs}s");
        }

        public static async Task TaskDelayAsync(int secs)
        {
            Logger.Log($"delaying by {secs}s");

            await Task.Delay(TimeSpan.FromSeconds(secs));

            Logger.Log($"finished delaying by {secs}s");
        }

        public static async Task TaskDelayAndThrowAsync(int secs)
        {
            await TaskDelayAsync(secs);

            throw new InvalidOperationException("some exception");
        }

        public static async void TaskDelayAndThrowAsyncVoid(int secs)
        {
            await TaskDelayAsync(secs);

            throw new InvalidOperationException("some exception");
        }

        public static async Task<string> TaskWithResult(string text)
        {
            await TaskDelayAsync(1);
            return text;
        }

        public static async Task TaskYieldThenWait()
        {
            await Task.Yield();
            TaskDelayAsync(1).Wait();
        }

        public static async Task TaskYieldThenDelay()
        {
            await Task.Yield();
            await TaskDelayAsync(1);
        }
    }
}