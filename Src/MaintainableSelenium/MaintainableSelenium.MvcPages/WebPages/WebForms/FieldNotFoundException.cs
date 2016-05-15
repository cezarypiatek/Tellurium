using MaintainableSelenium.MvcPages.SeleniumUtils;

namespace MaintainableSelenium.MvcPages.WebPages.WebForms
{
    public class FieldNotFoundException: WebElementNotFoundException
    {
        public FieldNotFoundException(string fieldName)
            : base(string.Format("Cannot find field with name '{0}'",fieldName))
        {
        }
    }
}