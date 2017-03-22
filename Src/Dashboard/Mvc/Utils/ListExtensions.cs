using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Tellurium.VisualAssertion.Dashboard.Mvc.Utils
{
    public static class ListExtensions
    {
        public static IReadOnlyList<T> AsReadonly<T>(this IList<T> list)
        {
            return new ReadOnlyCollection<T>(list);
        }
    }
}