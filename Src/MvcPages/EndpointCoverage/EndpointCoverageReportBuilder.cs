using System;
using System.Collections.Generic;
using System.Linq;

namespace Tellurium.MvcPages.EndpointCoverage
{
    public class EndpointCoverageReportBuilder
    {
        private readonly IEndpointCollector endpointCollector;
        private readonly IEndpointExplorer endpointExplorer;

        public EndpointCoverageReportBuilder(
            IEndpointCollector endpointCollector,
            IEndpointExplorer endpointExplorer)
        {
            this.endpointCollector = endpointCollector;
            this.endpointExplorer = endpointExplorer;
        }

        public void GenerateEndpointCoverageReport()
        {
            var visitedEndpoints = endpointCollector.GetAllRequestedEndpoints();
            var availableEndpoints = this.endpointExplorer.GetAvailableEndpoints().ToList();
            var coveragedEndpoints = availableEndpoints.Intersect(visitedEndpoints).ToList();
            var uncoverageEndpoints = availableEndpoints.Except(visitedEndpoints).ToList();
            Console.WriteLine($"Endpoints coverage: {coveragedEndpoints.Count()}/{availableEndpoints.Count}");
            PrintoutEndpointsList("Coveraged endpoints", coveragedEndpoints);
            PrintoutEndpointsList("Uncoveraged endpoints", uncoverageEndpoints);
        }

        private void PrintoutEndpointsList(string label, IReadOnlyList<string> endpoints)
        {
            Console.WriteLine($"{label}:");
            foreach (var endpoint in endpoints)
            {
                Console.WriteLine($"\t -> {endpoint}");
            }
        }
    }

    public interface IEndpointCollector
    {
        IReadOnlyCollection<string> GetAllRequestedEndpoints();
    }

    public interface IEndpointExplorer
    {
        IEnumerable<string> GetAvailableEndpoints();
    }
}