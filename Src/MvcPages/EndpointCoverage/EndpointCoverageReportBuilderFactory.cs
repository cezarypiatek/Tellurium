using Tellurium.MvcPages.Configuration;
using Tellurium.MvcPages.EndpointCoverage.EndpointExplorers;

namespace Tellurium.MvcPages.EndpointCoverage
{
    public static class EndpointCoverageReportBuilderFactory
    {
        public static EndpointCoverageReportBuilder Create(BrowserAdapterConfig config, IEndpointCollector endpointCollector)
        {
            if (config.MeasureEndpointCoverage == false)
            {
                return null;
            }
            var endpointExplorer = EndpointExplorerFactory.Create(config);
            return new EndpointCoverageReportBuilder(endpointCollector, endpointExplorer);
        }
    }
}