using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Examples.Helpers;

namespace Examples
{
    public static class Scenarios
    {
        /// <summary>
        /// It ilustrates how async / await breaks method execution on await and returns to parent method. It shows also that between task start and end it is possible to execute other tasks (see foo task).
        /// Finally, depending on the context, all tasks may execute on different threads but do not have to.
        /// </summary>
        public static async Task Method_execution_flow_example()
        {
            await Task.Yield();
            Logger.Log($"after {nameof(Task.Yield)}");

            var foo = Methods.Foo();
            Logger.Log($"after {nameof(Methods.Foo)}");

            await Methods.Bar(1);
            Logger.Log($"after {nameof(Methods.Bar)}");

            await foo;
        }

        /// <summary>
        /// It illustrates that while Thread.Sleep() executes on the same thread, the method after Task.Delay() may continue on different one.
        /// </summary>
        public static async Task Thread_Sleep_vs_Task_Delay()
        {
            Logger.Log("sleeping");
            Thread.Sleep(TimeSpan.FromSeconds(1));
            Logger.Log("finished sleeping");


            Logger.Log("delaying");
            await Task.Delay(TimeSpan.FromSeconds(1));
            Logger.Log("finished delaying");
        }


        /// <summary>
        /// It illustrates that await pauses method execution until awaited task finish.
        /// </summary>
        public static async Task Multiple_Task_Delay_executed_consecutively()
        {
            await Methods.TaskDelayAsync(1);
            await Methods.TaskDelayAsync(1);
        }

        /// <summary>
        /// It illustrates asynchronous execution of tasks.
        /// </summary>
        public static async Task Asynchronous_execution_of_tasks()
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
        public static async Task Multiple_Task_Delay_executed_with_When_All()
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
        /// It ilustrates that When.Any allows to effectively process subtasks as soon as they return
        /// </summary>
        public static async Task Simple_looping_vs_Task_WhenAny()
        {
            Logger.Log("simple loop");

            var tasks = new List<Task<string>>
            {
                Methods.TaskDelayThenReturn(3,"text1"),
                Methods.TaskDelayThenReturn(1, "text2"),
                Methods.TaskDelayThenReturn(2, "text3")
            };

            foreach (var task in tasks)
                Logger.Log($"finished {await task}");




            Logger.Log("now with Task.Any");

            tasks = new List<Task<string>>
            {
                Methods.TaskDelayThenReturn(3,"text1"),
                Methods.TaskDelayThenReturn(1, "text2"),
                Methods.TaskDelayThenReturn(2, "text3")
            };

            while (tasks.Count > 0)
            {
                var finishedTask = await Task.WhenAny(tasks);
                Logger.Log($"finished {await finishedTask}");
                tasks.Remove(finishedTask);
            }
        }

        /// <summary>
        /// It ilustrates that tasks long running tasks can be sliced with Task.Yield(), allowing to execute the long running bit in background, however it depends on the synchronization context.
        /// </summary>
        /// <returns></returns>
        public static async Task Processing_long_running_tasks()
        {
            var task1 = Methods.LongRunningTaskAsync();
            var task2 = Methods.YieldAndExecuteLongRunningTaskAsync();
            Logger.Log("awaiting task1");
            await task1;
            Logger.Log("awaiting task2");
            await task2;

            Logger.Log("waiting for all");
            await Task.WhenAll(
                Methods.YieldAndExecuteLongRunningTaskAsync(),
                Methods.YieldAndExecuteLongRunningTaskAsync());
        }

        /// <summary>
        /// It illustrates that await only allows to get the result or wait for task to finish, but it is not required for task to perform execution.
        /// See that scenario will finish before the executed task!
        /// </summary>
        public static void Not_awaited_tasks_will_still_finish()
        {
            Methods.TaskDelayAsync(1);
        }

        /// <summary>
        /// It illustrates that if async method is not awaited, we are loosing possibility to handle exceptions
        /// </summary>
        public static void Not_awaited_tasks_and_exceptions()
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
        public static async Task Await_and_exceptions()
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
        public static void Async_void_is_like_unawaited_task_but_it_may_kill_application()
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
        public static async Task Task_Result_and_Task_Wait_blocks_threads_and_may_cause_deadlocks()
        {
            Logger.Log("Performing Wait and Result...");
            Methods.TaskDelayAsync(1).Wait();
            Methods.TaskDelayAsync(1).Wait();
            Methods.TaskDelayAsync(1).Wait();
            Logger.Log(Methods.TaskDelayThenReturn(1, "abc").Result);

            Logger.Log("Performing await...");
            await Methods.TaskDelayAsync(1);
            await Methods.TaskDelayAsync(1);
            await Methods.TaskDelayAsync(1);
            Logger.Log(await Methods.TaskDelayThenReturn(1, "abc"));
        }

        /// <summary>
        /// It illustrates why mixing async with synchronous waiting should be avoided
        /// </summary>
        public static void Task_Wait_may_lead_to_severe_performance_issues_or_cause_deadlock()
        {
            Methods.TaskYieldThenWait().Wait();
            Logger.Log("survived");

            var tasks = new List<Task>();
            for (int i = 0; i < 1000; ++i)
                tasks.Add(Methods.TaskYieldThenWait());

            Task.WhenAll(tasks).Wait();
            Logger.Log("survived");
        }

        /// <summary>
        /// It illustrates that same scenario with async/await works
        /// </summary>
        public static async Task Awaiting_large_amount_of_tasks()
        {
            await Methods.TaskYieldThenDelay();
            Logger.Log("survived");

            var tasks = new List<Task>();
            for (int i = 0; i < 1000; ++i)
                tasks.Add(Methods.TaskYieldThenDelay());

            await Task.WhenAll(tasks);
            Logger.Log("survived");
        }

        /// <summary>
        /// It ilustrates that ConfigureAwait() method can be used on tasks to do not capture the synchronization context, which would allow to execute the task continuation on another thread if needed.
        /// </summary>
        public static void ConfigureAwait_example()
        {
            Methods.CallBlockingTaskWithConfigureAwait(false).Wait();
            Logger.Log("survived");
            Methods.CallBlockingTaskWithConfigureAwait(true).Wait();
            Logger.Log("survived");
        }
    }
}