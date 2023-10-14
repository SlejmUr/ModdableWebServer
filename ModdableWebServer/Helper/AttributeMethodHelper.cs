using ModdableWebServer.Attributes;
using System.Reflection;

namespace ModdableWebServer.Helper
{
    public static class AttributeMethodHelper
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
                DebugPrinter.Debug($"[UrlHTTPLoader] {httpAttr.method}, {httpAttr.url} = {method}");
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
                DebugPrinter.Debug($"[UrlWSLoader] {httpAttr.url} = {method}");
                ret.Add(httpAttr.url, method);
            }

            return ret;
        }

        public static void Merge(this Dictionary<(string url, string method), MethodInfo> keyValues, Assembly? assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));

            var methods = assembly.GetTypes().SelectMany(x => x.GetMethods()).ToArray();
            var basemethods = methods.Where(x => x.GetCustomAttribute<HTTPAttribute>() != null && x.ReturnType == typeof(bool)).ToArray();
            foreach (var method in basemethods)
            {
                if (method == null)
                    continue;
                var httpAttr = method.GetCustomAttribute<HTTPAttribute>();
                if (httpAttr == null)
                    continue;
                if (!keyValues.TryAdd((httpAttr.url, httpAttr.method), method))
                { 
                    Console.WriteLine($"Cannot merge {httpAttr.method} ,{httpAttr.url}.");
                }
                else
                    DebugPrinter.Debug($"[Merge HTTP] {httpAttr.method}, {httpAttr.url} = {method}");
            }
        }


        public static void Merge(this Dictionary<string, MethodInfo> keyValues, Assembly? assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));

            var methods = assembly.GetTypes().SelectMany(x => x.GetMethods()).ToArray();
            var basemethods = methods.Where(x => x.GetCustomAttribute<WSAttribute>() != null && x.ReturnType == typeof(void)).ToArray();
            foreach (var method in basemethods)
            {
                if (method == null)
                    continue;
                var httpAttr = method.GetCustomAttribute<WSAttribute>();
                if (httpAttr == null)
                    continue;
                if (!keyValues.TryAdd(httpAttr.url, method))
                {
                    Console.WriteLine($"Cannot merge {httpAttr.url}.");
                }
                else
                    DebugPrinter.Debug($"[Merge WS] {httpAttr.url} = {method}");
            }
        }


        public static void Override(this Dictionary<(string url, string method), MethodInfo> keyValues, Assembly? assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));

            var methods = assembly.GetTypes().SelectMany(x => x.GetMethods()).ToArray();
            var basemethods = methods.Where(x => x.GetCustomAttribute<HTTPAttribute>() != null && x.ReturnType == typeof(bool)).ToArray();
            foreach (var method in basemethods)
            {
                if (method == null)
                    continue;
                var httpAttr = method.GetCustomAttribute<HTTPAttribute>();
                if (httpAttr == null)
                    continue;

                if (keyValues.ContainsKey((httpAttr.url, httpAttr.method)))
                    keyValues.Remove((httpAttr.url, httpAttr.method));
                if (!keyValues.TryAdd((httpAttr.url, httpAttr.method), method))
                {
                    throw new Exception("KeyValues adding was not possible (Even after removed old data)");
                }
                else
                    DebugPrinter.Debug($"[Override HTTP] {httpAttr.method}, {httpAttr.url} = {method}");
            }
        }

        public static void Override(this Dictionary<string, MethodInfo> keyValues, Assembly? assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));

            var methods = assembly.GetTypes().SelectMany(x => x.GetMethods()).ToArray();
            var basemethods = methods.Where(x => x.GetCustomAttribute<WSAttribute>() != null && x.ReturnType == typeof(void)).ToArray();
            foreach (var method in basemethods)
            {
                if (method == null)
                    continue;
                var httpAttr = method.GetCustomAttribute<WSAttribute>();
                if (httpAttr == null)
                    continue;

                if (keyValues.ContainsKey(httpAttr.url))
                    keyValues.Remove(httpAttr.url);
                if (!keyValues.TryAdd(httpAttr.url, method))
                {
                    throw new Exception("KeyValues adding was not possible (Even after removed old data)");
                }
                else
                    DebugPrinter.Debug($"[Override WS] {httpAttr.url} = {method}");
            }
        }
    }
}
