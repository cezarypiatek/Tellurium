using System;

namespace Tellurium.MvcPages.WebPages
{
    public class EmptyWebElementCollectionException : ApplicationException
    {
        public EmptyWebElementCollectionException() : base("List is empty")
        {
        }
    }
}