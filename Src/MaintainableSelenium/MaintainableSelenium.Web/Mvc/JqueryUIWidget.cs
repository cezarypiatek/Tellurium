using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace MaintainableSelenium.Web.Mvc
{
    /// <summary>
    /// Base class for jquery ui widget which will be registered using modernUnobtrusive library
    /// </summary>
    public abstract class JqueryUIWidget
    {
        private readonly string widgetName;

        private Dictionary<string, string> options;

        protected JqueryUIWidget(string widgetName)
        {
            this.widgetName = widgetName;
            options = new Dictionary<string, string>();
        }

        protected void SetOption(string optionName, object value)
        {
            var optionKey = string.Format("data-ui-{0}-{1}", widgetName, optionName);
            options[optionKey] = value == null?"": value.ToString();
        }

        protected void SetComplexOption(string optionName, object optionObj)
        {
            var serializedOption = JsonConvert.SerializeObject(optionObj);
            SetOption(optionName, serializedOption);
        }

        public Dictionary<string,string> GetWidgetRegistrationAttributes()
        {
            var registrationAttributes = options.ToDictionary(x => x.Key, x => x.Value);
            registrationAttributes.Add("data-ui-fn", widgetName);
            return registrationAttributes;
        }

        public IHtmlString GetAttributeString()
        {
            var sb = new StringBuilder();
            foreach (var attribute in GetWidgetRegistrationAttributes())
            {
                sb.AppendFormat("{0}=\"{1}\"", attribute.Key, attribute.Value);
            }
            return MvcHtmlString.Create(sb.ToString());
        }
    }
}