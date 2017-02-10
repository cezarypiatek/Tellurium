using System;

namespace Tellurium.MvcPages.WebPages.WebForms
{
    public class UnableToSetFieldValueException:ApplicationException
    {
        public UnableToSetFieldValueException(string fieldName, string value)
            : base($"Cannot set value '{value}' for field '{fieldName}'")
        {
        }
    }
}