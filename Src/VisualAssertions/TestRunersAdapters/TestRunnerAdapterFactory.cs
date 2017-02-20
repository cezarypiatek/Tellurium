using System;

using Tellurium.VisualAssertions.TestRunersAdapters.Providers;

namespace Tellurium.VisualAssertions.TestRunersAdapters
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