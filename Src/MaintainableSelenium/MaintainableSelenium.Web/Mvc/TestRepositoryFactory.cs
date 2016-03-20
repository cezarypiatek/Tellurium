using MaintainableSelenium.Toolbox.Screenshots;

namespace MaintainableSelenium.Web.Mvc
{
    public static class TestRepositoryFactory
    {
        public static ITestRepository Create()
        {
            return new FileTestStorage(@"c:\MaintainableSelenium\screenshots");
        }
    }
}