using System.Collections.Generic;

namespace Tellurium.MvcPages.EndpointCoverage.EndpointExplorers
{
    public class CompositeEndpointExplorer:IEndpointExplorer
    {
        private readonly IEndpointExplorer[] endpointsExplorers;

        public CompositeEndpointExplorer(params IEndpointExplorer[] endpointsExplorers)
        {
            this.endpointsExplorers = endpointsExplorers;
        }

        public IEnumerable<string> GetAvailableEndpoints()
        {
            foreach (var endpointsExplorer in endpointsExplorers)
            {
                foreach (var endpoint in endpointsExplorer.GetAvailableEndpoints())
                {
                    yield return endpoint;
                }
            }
        }
    }
}