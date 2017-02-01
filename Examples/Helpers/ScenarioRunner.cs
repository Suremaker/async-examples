using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Exmaples.Helpers
{
    public class ScenarioRunner
    {
        private readonly MethodInfo _scenario;

        public string Name => ToNiceName();

        public async Task RunScenarioAsync()
        {
            Logger.Init(Name);

            Logger.LogStart(_scenario.Name);
            var task = _scenario.Invoke(null, null) as Task;
            if (task != null)
                await task;
            Logger.LogEnd(_scenario.Name);
        }

        public static ScenarioRunner[] FindScenarios(Type type)
        {
            return type.GetMethods(BindingFlags.Static | BindingFlags.Public).Select(m => new ScenarioRunner(m)).ToArray();
        }

        private ScenarioRunner(MethodInfo scenario)
        {
            _scenario = scenario;
        }

        private string ToNiceName()
        {
            return _scenario.Name.Replace('_', ' ');
        }
    }
}