using System;

namespace Tellurium.MvcPages.WebPages.WebForms
{
    public class UnableToSetFieldValueException:ApplicationException
    {
        public UnableToSetFieldValueException(string fieldDescription, string value, Exception originalException)
            : base($"Cannot set value '{value}' for '{fieldDescription}'", originalException)
        {
        }
    }
}