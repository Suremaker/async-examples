using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Helpers
{
    public class Logger
    {
        private static readonly Stopwatch _watch = new Stopwatch();

        public static void Init(string scenarioName)
        {
            _watch.Restart();
            WriteText($"\nScenario: {scenarioName}");
        }

        public static void Log(string message, [CallerMemberName]string caller = null)
        {
            var text = $"Elapsed: {_watch.ElapsedMilliseconds} ms Thread: {Thread.CurrentThread.ManagedThreadId}> {caller}: {message} (SynchronizationContext: {SynchronizationContext.Current?.ToString() ?? "none"})";
            WriteText(text);
        }

        public static void LogStart([CallerMemberName]string caller = null)
        {
            Log("start", caller);
        }

        public static void LogEnd([CallerMemberName]string caller = null)
        {
            Log("end", caller);
        }

        private static void WriteText(string text)
        {
            Console.WriteLine(text);
            Trace.WriteLine(text);
        }
    }
}
