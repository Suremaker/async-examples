using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Examples.Helpers
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
            var text = $"{_watch.ElapsedMilliseconds}ms Thread {Thread.CurrentThread.ManagedThreadId} (context: {SynchronizationContext.Current?.GetType().Name ?? "none"})> {caller}: {message}";
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
            Trace.WriteLine(text);
        }
    }
}
