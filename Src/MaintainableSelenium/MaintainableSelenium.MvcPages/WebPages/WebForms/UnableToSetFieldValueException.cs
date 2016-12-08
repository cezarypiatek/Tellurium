using System;

namespace MaintainableSelenium.MvcPages.WebPages.WebForms
{
    public class UnableToSetFieldValueException:ApplicationException
    {
        public UnableToSetFieldValueException(string fieldName, string value)
            : base(string.Format("Cannot set value '{0}' for field '{1}'",  value, fieldName))
        {
        }
    }
}