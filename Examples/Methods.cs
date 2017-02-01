using System;
using System.Threading;
using System.Threading.Tasks;
using Examples.Helpers;

internal static class Methods
{
    public static async Task Bar(int i)
    {
        Logger.Log($"start {i}");

        await Task.Delay(TimeSpan.FromMilliseconds(100));
        Logger.Log($"end {i}");
    }

    public static async Task Foo()
    {
        Logger.LogStart();

        await Task.Delay(TimeSpan.FromMilliseconds(200));
        Logger.Log("after delay");

        await Bar(2);
        Logger.LogEnd();
    }

    public static async Task TaskDelayAsync(int secs)
    {
        Logger.Log($"delaying by {secs}s");

        await Task.Delay(TimeSpan.FromSeconds(secs));

        Logger.Log($"finished delaying by {secs}s");
    }

    public static async Task<string> TaskDelayThenReturn(int secs, string text)
    {
        Logger.Log($"start {text}");
        await Task.Delay(TimeSpan.FromSeconds(secs));
        Logger.Log($"finished {text}");
        return text;
    }

    public static async void TaskDelayAndThrowAsyncVoid(int secs)
    {
        await TaskDelayAsync(secs);

        throw new InvalidOperationException("some exception");
    }

    public static async Task TaskYieldThenWait()
    {
        Logger.LogStart();
        await Task.Yield();

        Logger.Log("waiting");
        Task.Delay(TimeSpan.FromSeconds(5)).Wait();
        Logger.LogEnd();
    }

    public static async Task TaskYieldThenDelay()
    {
        Logger.LogStart();
        await Task.Yield();

        Logger.Log("awaiting");
        await Task.Delay(TimeSpan.FromSeconds(5));
        Logger.LogEnd();
    }

    public static async Task TaskDelayAndThrowAsync(int secs)
    {
        await TaskDelayAsync(secs);

        throw new InvalidOperationException("some exception");
    }

    public static Task LongRunningTaskAsync() 
    {
        Logger.LogStart();
        Thread.Sleep(TimeSpan.FromSeconds(1)); //emulate long running task
        Logger.LogEnd();
        return Task.CompletedTask;
    }

    public static async Task YieldAndExecuteLongRunningTaskAsync()
    {
        Logger.LogStart();
        await Task.Yield();
        Thread.Sleep(TimeSpan.FromSeconds(1)); //emulate long running task
        Logger.LogEnd();
    }

    public static async Task CallBlockingTaskWithConfigureAwait(bool continueOnCapturedContext)
    {
        Logger.Log($"start with {nameof(continueOnCapturedContext)}={continueOnCapturedContext}");
        await Task.Delay(TimeSpan.FromSeconds(1)).ConfigureAwait(continueOnCapturedContext);
        Logger.Log($"end with {nameof(continueOnCapturedContext)}={continueOnCapturedContext}");
    }
}