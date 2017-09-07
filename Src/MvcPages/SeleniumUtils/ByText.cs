using OpenQA.Selenium;
using OpenQA.Selenium.Internal;
using Tellurium.MvcPages.Utils;

namespace Tellurium.MvcPages.SeleniumUtils
{
    public class ByText:By
    {
        public static ByText FromPartial(string text)
        {
            var xpathLiteral = XPathHelpers.ToXPathLiteral(text.Trim());
            var xpathToFind = string.Format(".//*[contains(text(), {0}) or ((@type='submit' or  @type='reset') and contains(@value,{0})) or contains(@title,{0})]", xpathLiteral);
            return new ByText(xpathToFind, $"With partial text: '{text}'");
        }
        
        public static ByText From(string text)
        {
            var xpathLiteral = XPathHelpers.ToXPathLiteral(text.Trim());
            var xpathToFind = string.Format(".//*[((normalize-space(.) = {0}) and (count(*)=0) ) or (normalize-space(text()) = {0}) or ((@type='submit' or  @type='reset') and @value={0}) or (@title={0})]", xpathLiteral);
            return new ByText(xpathToFind, $"With text: '{text}'");
        }

        private ByText(string xpathToFind, string description)
        {
            FindElementMethod = context => ((IFindsByXPath) context).FindElementByXPath(xpathToFind);
            FindElementsMethod = context => ((IFindsByXPath) context).FindElementsByXPath(xpathToFind);
            Description = description;
        }
    }
}