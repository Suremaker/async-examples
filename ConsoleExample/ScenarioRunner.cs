using System;
using System.Reflection;
using System.Threading.Tasks;
using Helpers;

namespace ConsoleExample
{
    static class ScenarioRunner
    {
        public static void RunScenarios(Type type)
        {
            var scenarios = type.GetMethods(BindingFlags.Static | BindingFlags.Public);
            while (true)
            {
                Console.WriteLine("\nSelect scenario to run: ");
                for (var index = 0; index < scenarios.Length; index++)
                    Console.WriteLine($"{index}: {ToNiceName(scenarios[index])}");
                Console.Write("> ");

                var selection = Console.ReadLine();
                uint selectedScenarioIndex;
                if (uint.TryParse(selection, out selectedScenarioIndex) && selectedScenarioIndex < scenarios.Length)
                    InvokeScenario(scenarios[selectedScenarioIndex]);
            }
        }

        private static void InvokeScenario(MethodInfo scenario)
        {
            Logger.Init(ToNiceName(scenario));

            Logger.LogStart(scenario.Name);
            var task = scenario.Invoke(null, null) as Task;
            task?.Wait();
            Logger.LogEnd(scenario.Name);
        }

        private static string ToNiceName(MethodInfo scenario)
        {
            return scenario.Name.Replace('_', ' ');
        }
    }
}