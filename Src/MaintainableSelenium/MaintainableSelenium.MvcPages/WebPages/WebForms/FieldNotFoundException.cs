using System;
using Tellurium.MvcPages.SeleniumUtils;

namespace Tellurium.MvcPages.WebPages.WebForms
{
    public class FieldNotFoundException: WebElementNotFoundException
    {
        public FieldNotFoundException(string fieldName)
            : base(string.Format("Cannot find field with name '{0}'",fieldName))
        {
        }
    }

    public class FieldNotAccessibleException:ApplicationException
    {
        public FieldNotAccessibleException(string fieldName)
            :base(string.Format("Cannot access field with name '{0}'",fieldName))
        {
        }
    }
}