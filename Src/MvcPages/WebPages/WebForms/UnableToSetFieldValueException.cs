using System;

namespace Tellurium.MvcPages.WebPages.WebForms
{
    public class UnableToSetFieldValueException:ApplicationException
    {
        public UnableToSetFieldValueException(string fieldName, string value, Exception originalException)
            : base($"Cannot set value '{value}' for field '{fieldName}'", originalException)
        {
        }
    }
}