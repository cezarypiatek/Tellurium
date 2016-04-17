namespace MaintainableSelenium.Toolbox.Screenshots.Domain
{
    public interface IBlindRegionForBrowserOwner
    {
        void AddBlindRegionForBrowser(BlindRegionForBrowser blindRegionForBrowser);
        BlindRegionForBrowser GetOwnBlindRegionForBrowser(string browserName);
    }
}