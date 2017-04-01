namespace Tellurium.MvcPages
{
    /// <summary>
    /// Ambient context for browser adapter. Temporary solution.
    /// </summary>
    internal static class BrowserAdapterContext
    {
        public static BrowserAdapter Current { get; set; }
    }
}