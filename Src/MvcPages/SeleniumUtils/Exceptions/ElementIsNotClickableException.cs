namespace Tellurium.MvcPages.SeleniumUtils.Exceptions
{
    public class ElementIsNotClickableException: CannotInteractWithElementException
    {
        public ElementIsNotClickableException():base("Element  is not clickable")
        {
            
        }
    }
}