using System;

namespace MaintainableSelenium.Toolbox.Screenshots
{
    public static class IdGenerator
    {
        public static string GetNewId()
        {
            return Guid.NewGuid().ToString().Replace("-", "");
        }
    }
}