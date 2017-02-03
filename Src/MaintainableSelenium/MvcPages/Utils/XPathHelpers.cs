using System.Text;

namespace Tellurium.MvcPages.Utils
{
    internal static class XPathHelpers
    {
        //INFO: http://stackoverflow.com/questions/1341847/special-character-in-xpath-query
        public static string ToXPathLiteral(string value)
        {
            // if the value contains only single or double quotes, construct
            // an XPath literal
            if (!value.Contains("\""))
            {
                return "\"" + value + "\"";
            }
            if (!value.Contains("'"))
            {
                return "'" + value + "'";
            }

            // if the value contains both single and double quotes, construct an
            // expression that concatenates all non-double-quote substrings with
            // the quotes, e.g.:
            //
            //    concat("foo", '"', "bar")
            StringBuilder sb = new StringBuilder();
            sb.Append("concat(");
            string[] substrings = value.Split('\"');
            for (int i = 0; i < substrings.Length; i++)
            {
                bool needComma = (i > 0);
                if (substrings[i] != "")
                {
                    if (i > 0)
                    {
                        sb.Append(", ");
                    }
                    sb.Append("\"");
                    sb.Append(substrings[i]);
                    sb.Append("\"");
                    needComma = true;
                }
                if (i < substrings.Length - 1)
                {
                    if (needComma)
                    {
                        sb.Append(", ");
                    }
                    sb.Append("'\"'");
                }

            }
            sb.Append(")");
            return sb.ToString();
        }
    }
}