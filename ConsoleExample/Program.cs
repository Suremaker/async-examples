using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Helpers;

namespace ConsoleExample
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

    class Program
    {
        static void Main(string[] args)
        {
            ScenarioRunner.RunScenarios(typeof(Program));
        }

        /// <summary>
        /// It illustrates that while Thread.Sleep() executes on the same thread, the method after Task.Delay() may continue on different one.
        /// </summary>
        public static async Task E0_Thread_Sleep_vs_Task_Delay()
        {
            Methods.ThreadSleep(1);
            await Methods.TaskDelayAsync(1);
        }

        /// <summary>
        /// It illustrates that await pauses method execution until awaited task finish.
        /// </summary>
        public static async Task E1_multiple_Task_Delay_executed_consecutively()
        {
            await Methods.TaskDelayAsync(1);
            await Methods.TaskDelayAsync(1);
        }

        /// <summary>
        /// It illustrates asynchronous execution of tasks.
        /// </summary>
        public static async Task E2_asynchronous_execution_of_tasks()
        {
            var task1 = Methods.TaskDelayAsync(1);
            Logger.Log("after task1");
            var task2 = Methods.TaskDelayAsync(3);
            Logger.Log("after task2");
            var task3 = Methods.TaskDelayAsync(2);
            Logger.Log("after task3");

            await task1;
            Logger.Log("awaited task1");
            await task2;
            Logger.Log("awaited task2");
            await task3;
            Logger.Log("awaited task3");
        }

        /// <summary>
        /// It illustrates that When.All() allows to execute methods asynchronously, returning when all finish
        /// </summary>
        public static async Task E3_multiple_Task_Delay_executed_with_When_All()
        {
            var task1 = Methods.TaskDelayAsync(1);
            Logger.Log("after task1");
            var task2 = Methods.TaskDelayAsync(3);
            Logger.Log("after task2");
            var task3 = Methods.TaskDelayAsync(2);
            Logger.Log("after task3");

            await Task.WhenAll(task1, task2, task3);
            Logger.Log("after wait all");
        }

        /// <summary>
        /// It illustrates that await only allows to get the result or wait for task to finish, but it is not required for task to perform execution.
        /// See that scenario will finish before the executed task!
        /// </summary>
        public static void E4_not_awaited_tasks_still_will_finish()
        {
            Methods.TaskDelayAsync(1);
        }

        /// <summary>
        /// It illustrates that if async method is not awaited, we are loosing possibility to handle exceptions
        /// </summary>
        public static void E5_not_awaited_tasks_and_exceptions()
        {
            try
            {
                Methods.TaskDelayAndThrowAsync(1);
                Logger.Log("no exception!");
            }
            catch (Exception e)
            {
                Logger.Log($"Exception caught: {e.Message}");
            }
        }

        /// <summary>
        /// It illustrates that await allows to handle exceptions
        /// </summary>
        public static async Task E6_await_and_exceptions()
        {
            try
            {
                await Methods.TaskDelayAndThrowAsync(1);
            }
            catch (Exception e)
            {
                Logger.Log($"Exception caught: {e.Message}");
            }
        }

        /// <summary>
        /// It illustrates that if async method is not awaited, we are loosing possibility to handle exceptions, which may end badly :)
        /// </summary>
        public static void E7_async_void_is_like_unawaited_task()
        {
            try
            {
                Methods.TaskDelayAndThrowAsyncVoid(1);
                Logger.Log("no exception!");
            }
            catch (Exception e)
            {
                Logger.Log($"Exception caught: {e.Message}");
            }
        }

        /// <summary>
        /// It illustrates that any call to Task.Result or Task.Wait() blocks current thread making application ineffective.
        /// </summary>
        public static async Task E8_Task_Result_and_Task_Wait_blocks_threads()
        {
            Methods.TaskDelayAsync(1).Wait();
            Methods.TaskDelayAsync(1).Wait();
            Methods.TaskDelayAsync(1).Wait();
            Logger.Log(Methods.TaskWithResult("abc").Result);

            await Methods.TaskDelayAsync(1);
            await Methods.TaskDelayAsync(1);
            await Methods.TaskDelayAsync(1);
            Logger.Log(await Methods.TaskWithResult("abc"));
        }

        /// <summary>
        /// It illustrates why mixing async with synchronous waiting should be avoided
        /// </summary>
        public static async Task E9_Task_Wait_may_lead_to_severe_performance_issues()
        {
            await Methods.TaskYieldThenWait();
            Logger.Log("survived");

            var tasks = new List<Task>();
            for (int i = 0; i < 1000; ++i)
                tasks.Add(Methods.TaskYieldThenWait());
            await Task.WhenAll(tasks);
            Logger.Log("survived");
        }

        /// <summary>
        /// It illustrates that same scenario with async/await works
        /// </summary>
        public static async Task E10_awaiting_large_amount_of_tasks()
        {
            await Methods.TaskYieldThenDelay();
            Logger.Log("survived");

            var tasks = new List<Task>();
            for (int i = 0; i < 1000; ++i)
                tasks.Add(Methods.TaskYieldThenDelay());
            await Task.WhenAll(tasks);
            Logger.Log("survived");
        }
    }
}
