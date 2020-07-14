using System;
using Tellurium.VisualAssertions.TestRunnersAdapters.Providers;

namespace Tellurium.VisualAssertions.TestRunnersAdapters
{
    internal static class TestRunnerAdapterFactory
    {

        public static ITestRunnerAdapter CreateForCurrentEnvironment(Action<string> testOuputWriter)
        {
            if (TeamCityRunnerAdapter.IsAvailable())
            {
                return new TeamCityRunnerAdapter(testOuputWriter);
            }
            return new ConsoleTestRunnerAdapter(testOuputWriter);
        }
    }
}