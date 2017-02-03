using System.Collections.Generic;
using System.Linq;
using Tellurium.VisualAssertions.TestRunersAdapters.Providers;

namespace Tellurium.VisualAssertions.TestRunersAdapters
{
    public static class TestRunnerAdapterFactory
    {
        private static IList<ITestRunnerAdapter> AvilableTestRunnerAdapters = new List<ITestRunnerAdapter>()
        {
            new TeamCityRunnerAdapter(),
            new ConsoleTestRunnerAdapter()
        };

        public static ITestRunnerAdapter CreateForCurrentEnvironment()
        {
            return AvilableTestRunnerAdapters.First(x => x.IsPresent());
        }
    }
}