using System;
using Exmaples;
using Exmaples.Helpers;

namespace ConsoleExample
{
    class Program
    {
        static void Main(string[] args)
        {
            var scenarios = ScenarioRunner.FindScenarios(typeof(Scenarios));

            while (true)
            {
                Console.WriteLine("\nSelect scenario to run: ");
                for (var index = 0; index < scenarios.Length; index++)
                    Console.WriteLine($"{index}: {scenarios[index].Name}");
                Console.Write("> ");

                var selection = Console.ReadLine();
                uint selectedScenarioIndex;
                if (uint.TryParse(selection, out selectedScenarioIndex) && selectedScenarioIndex < scenarios.Length)
                    scenarios[selectedScenarioIndex].RunScenarioAsync().Wait();
            }
        }
    }
}
