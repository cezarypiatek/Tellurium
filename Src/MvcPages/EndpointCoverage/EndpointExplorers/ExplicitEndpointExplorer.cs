using System.Collections.Generic;

namespace Tellurium.MvcPages.EndpointCoverage.EndpointExplorers
{
    public class ExplicitEndpointExplorer:IEndpointExplorer
    {
        private readonly IReadOnlyList<string> knownEndpoints;

        public ExplicitEndpointExplorer(IReadOnlyList<string> knownEndpoints)
        {
            this.knownEndpoints = knownEndpoints;
        }

        public IEnumerable<string> GetAvailableEndpoints()
        {
            return this.knownEndpoints;
        }
    }
}