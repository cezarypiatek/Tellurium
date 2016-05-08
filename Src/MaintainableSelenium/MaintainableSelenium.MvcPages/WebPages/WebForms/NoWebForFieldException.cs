using System;

namespace MaintainableSelenium.MvcPages.WebPages.WebForms
{
    public class NoWebForFieldException: ApplicationException
    {
        public NoWebForFieldException(string fieldName)
            : base(string.Format("Cannot find element with name '{0}'",fieldName))
        {
        }
    }
}