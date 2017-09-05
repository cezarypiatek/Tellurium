using OpenQA.Selenium;

namespace Tellurium.MvcPages.SeleniumUtils.Exceptions
{
    public class ElementIsNotClickableException: CannotInteractWithElementException
    {
       public ElementIsNotClickableException(IWebElement element):
            base($"Element {element.GetElementDescription()} is not clickable")
        {
            
        }
    }
}