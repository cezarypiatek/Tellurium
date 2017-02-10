using System;
using System.Collections.Generic;
using System.Linq;

namespace Tellurium.MvcPages.EndpointCoverage
{
    public static class EndpointCoverageReportGenerator
    {
        public static void GenerateEndpointCoverageReport(IReadOnlyCollection<string> availableEndpoints, IReadOnlyCollection<string> visitedEndpoints)
        {
            var coveragedEndpoints = availableEndpoints.Intersect(visitedEndpoints).ToList();
            var uncoverageEndpoints = availableEndpoints.Except(visitedEndpoints).ToList();
            Console.WriteLine($"Endpoints coverage: {coveragedEndpoints.Count()}/{availableEndpoints.Count}");
            PrintoutEndpointsList("Coveraged endpoints", coveragedEndpoints);
            PrintoutEndpointsList("Uncoveraged endpoints", uncoverageEndpoints);
        }

        private static void PrintoutEndpointsList(string label, IReadOnlyList<string> endpoints)
        {
            Console.WriteLine($"{label}:");
            foreach (var endpoint in endpoints)
            {
                Console.WriteLine($"\t -> {endpoint}");
            }
        }
    }
}