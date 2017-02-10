using System;
using Tellurium.MvcPages.SeleniumUtils;

namespace Tellurium.MvcPages.WebPages.WebForms
{
    public class FieldNotFoundException: WebElementNotFoundException
    {
        public FieldNotFoundException(string fieldName)
            : base($"Cannot find field with name '{fieldName}'")
        {
        }
    }

    public class FieldNotAccessibleException:ApplicationException
    {
        public FieldNotAccessibleException(string fieldName)
            :base($"Cannot access field with name '{fieldName}'")
        {
        }
    }
}