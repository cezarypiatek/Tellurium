using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MaintainableSelenium.VisualAssertions.Web.Mvc.Utils
{
    public static class ListExtensions
    {
        public static IReadOnlyList<T> AsReadonly<T>(this IList<T> list)
        {
            return new ReadOnlyCollection<T>(list);
        }
    }
}