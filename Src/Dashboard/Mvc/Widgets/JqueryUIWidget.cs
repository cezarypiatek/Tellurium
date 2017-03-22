using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Html;
using Newtonsoft.Json;

namespace Tellurium.VisualAssertion.Dashboard.Mvc.Widgets
{
    /// <summary>
    /// Base class for jquery ui widget which will be registered using modernUnobtrusive library
    /// </summary>
    public abstract class JqueryUIWidget
    {
        private readonly string widgetName;

        private readonly Dictionary<string, string> options;

        protected JqueryUIWidget(string widgetName)
        {
            this.widgetName = widgetName;
            options = new Dictionary<string, string>();
        }

        /// <summary>
        /// Set simple values as an option
        /// </summary>
        /// <param name="optionName">Option name</param>
        /// <param name="value">Option value</param>
        protected void SetOption(string optionName, object value)
        {
            var dashedOptionName = CamelCaseToDashSeparated(optionName).ToLower();
            var optionKey = string.Format("data-ui-{0}-{1}", widgetName, dashedOptionName);
            options[optionKey] = value == null?"": value.ToString();
        }

        /// <summary>
        /// Set complex object as an option
        /// </summary>
        /// <remarks>
        /// Option object will be serialized to JSON
        /// </remarks>
        /// <param name="optionName">Name for option</param>
        /// <param name="optionObj">Option object</param>
        protected void SetComplexOption(string optionName, object optionObj)
        {
            var serializedOption = JsonConvert.SerializeObject(optionObj);
            SetOption(optionName, System.Net.WebUtility.HtmlEncode(serializedOption));
        }

        /// <summary>
        /// Get collection of registration attributes
        /// </summary>
        public Dictionary<string,string> GetWidgetRegistrationAttributes()
        {
            var registrationAttributes = options.ToDictionary(x => x.Key, x => x.Value);
            registrationAttributes.Add("data-ui-fn", widgetName);
            return registrationAttributes;
        }

        /// <summary>
        /// Get <see cref="IHtmlString"/> object containing registration attributes
        /// </summary>
        public HtmlString GetAttributeString()
        {
            var sb = new StringBuilder();
            foreach (var attribute in GetWidgetRegistrationAttributes())
            {
                sb.AppendFormat("{0}=\"{1}\"", attribute.Key, attribute.Value);
            }
            return new HtmlString(sb.ToString());
        }

        private static string CamelCaseToDashSeparated(string optionName)
        {
            return Regex.Replace(optionName, "(?=[A-Z])", "-", RegexOptions.Compiled).Trim('-');
        }
    }
}