using System.Collections.Generic;
using System.Linq;
using Tellurium.MvcPages.Configuration;

namespace Tellurium.MvcPages.EndpointCoverage.EndpointExplorers
{
    public static class EndpointExplorerFactory
    {
        public static IEndpointExplorer Create(BrowserAdapterConfig config)
        {
            var endpointsExplorers = GetAvailableEndpointsExplorers(config).ToArray();
            return new CompositeEndpointExplorer(endpointsExplorers);
        }

        private static IEnumerable<IEndpointExplorer> GetAvailableEndpointsExplorers(BrowserAdapterConfig config)
        {
            if (config.AvailableEndpoints?.Count > 0)
            {
                yield return new ExplicitEndpointExplorer(config.AvailableEndpoints);
            }

            if (config.AvailableEndpointsAssemblies?.Count > 0)
            {
                yield return new AspMvcEndpointExplorer(config.AvailableEndpointsAssemblies);
            }
        }
    }
}