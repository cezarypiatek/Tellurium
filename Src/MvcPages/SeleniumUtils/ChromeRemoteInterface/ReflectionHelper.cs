using System.Reflection;

namespace Tellurium.MvcPages.SeleniumUtils.ChromeRemoteInterface
{
    internal static class ReflectionHelper
    {
        public static T GetProperty<T>(object obj, string property)
        {
            return (T) obj.GetType().GetProperty(property, BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(obj);
        }
    }
}