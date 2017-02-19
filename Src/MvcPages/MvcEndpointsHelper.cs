using System.Text.RegularExpressions;

namespace Tellurium.MvcPages
{
    internal static class MvcEndpointsHelper
    {
        public static string NormalizeEndpointAddress(string endpoint)
        {
            var address = endpoint.Replace("/Index", "").Trim('/');
            return IdPattern.Replace(address,"");
        }

        private static readonly Regex IdPattern = new Regex(@"/\d+$");
    }
}