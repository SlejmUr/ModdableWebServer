using ModdableWebServer.Attributes;
using System.Reflection;

namespace ModdableWebServer.Helper
{
    public static class AttibuteMethodHelper
    {
        public static Dictionary<(string url, string method), MethodInfo> UrlHTTPLoader(Assembly? assembly)
        {
            if (assembly == null)
                return new();

            Dictionary<(string url, string method), MethodInfo> ret = new();
            var methods = assembly.GetTypes().SelectMany(x => x.GetMethods()).ToArray();
            var basemethods = methods.Where(x => x.GetCustomAttribute<HTTPAttribute>() != null && x.ReturnType == typeof(bool)).ToArray();
            foreach (var method in basemethods)
            {
                if (method == null)
                    continue;
                var httpAttr = method.GetCustomAttribute<HTTPAttribute>();
                if (httpAttr == null)
                    continue;
                ret.Add((httpAttr.url, httpAttr.method), method);
            }

            return ret;
        }

        public static Dictionary<string, MethodInfo> UrlWSLoader(Assembly? assembly)
        {
            if (assembly == null)
                return new();

            Dictionary<string, MethodInfo> ret = new();
            var methods = assembly.GetTypes().SelectMany(x => x.GetMethods()).ToArray();
            var basemethods = methods.Where(x => x.GetCustomAttribute<WSAttribute>() != null && x.ReturnType == typeof(void)).ToArray();
            foreach (var method in basemethods)
            {
                if (method == null)
                    continue;
                var httpAttr = method.GetCustomAttribute<WSAttribute>();
                if (httpAttr == null)
                    continue;
                ret.Add(httpAttr.url, method);
            }

            return ret;
        }
    }
}
