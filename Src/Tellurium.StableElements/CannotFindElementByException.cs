using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;

namespace Tellurium.MvcPages.SeleniumUtils.Exceptions
{
    public class CannotFindElementByException: WebElementNotFoundException
    {
        public By By { get; }

        public ISearchContext Context { get; }

        public CannotFindElementByException(By @by, ISearchContext context, Exception originalException = null)
            :base($"Cannot find element {by} inside {context.GetElementDescription()}", originalException)
        {
            By = @by;
            Context = context;
        }
    }

    public class CannotFindAccessibleElementByException:CannotFindElementByException
    {
        private List<string> candidatesDescriptions;

        public IReadOnlyList<IWebElement> Candidates { get; }

        public CannotFindAccessibleElementByException(By @by, ISearchContext context, IReadOnlyList<IWebElement> candidates, Exception originalException = null) 
            : base(@by, context, originalException)
        {
            Candidates = candidates;
            this.candidatesDescriptions = GetCandidatesDescriptions();
        }

        public override string Message
        {
            get
            {
                if (Candidates == null || Candidates.Count == 0)
                {
                    return base.Message;
                }
                return $"{base.Message} which meets accessibility criteria. Potential candidates:{string.Join("", candidatesDescriptions)}";
            }
        }

        private List<string> GetCandidatesDescriptions()
        {
            return Candidates?.Select(x=>
            {
                try
                {
                    return $"\r\n\t- {x.TagName}[Displayed={x.Displayed}, Enabled={x.Enabled}]";
                }
                catch
                {
                    return null;
                }
                    
            }).Where(x=> string.IsNullOrWhiteSpace(x) == false).ToList();
        }
    }
}