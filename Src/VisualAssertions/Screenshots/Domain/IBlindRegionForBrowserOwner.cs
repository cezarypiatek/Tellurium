namespace Tellurium.VisualAssertions.Screenshots.Domain
{
    public interface IBlindRegionForBrowserOwner
    {
        void AddBlindRegionForBrowser(BlindRegionForBrowser blindRegionForBrowser);
        BlindRegionForBrowser GetOwnBlindRegionForBrowser(string browserName);
    }
}