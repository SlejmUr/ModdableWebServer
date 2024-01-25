using ModdableWebServer.Attributes;
using System.Collections.Generic;
using System.Reflection;

namespace ModdableWebServer.Helper
{
    public static class AttributeMethodHelper
    {
        public static Dictionary<HTTPAttribute, MethodInfo> UrlHTTPLoader(Assembly? assembly)
        {
            if (assembly == null)
                return new();

            Dictionary<HTTPAttribute, MethodInfo> ret = new();
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
                ret.Add(httpAttr, method);
            }

            return ret;
        }


        public static Dictionary<HTTPHeaderAttribute, MethodInfo> UrlHTTPHeaderLoader(Assembly? assembly)
        {
            if (assembly == null)
                return new();

            Dictionary<HTTPHeaderAttribute, MethodInfo> ret = new();
            var methods = assembly.GetTypes().SelectMany(x => x.GetMethods()).ToArray();
            var basemethods = methods.Where(x => x.GetCustomAttribute<HTTPHeaderAttribute>() != null && x.ReturnType == typeof(bool)).ToArray();
            foreach (var method in basemethods)
            {
                if (method == null)
                    continue;
                var httpAttr = method.GetCustomAttribute<HTTPHeaderAttribute>();
                if (httpAttr == null)
                    continue;
                DebugPrinter.Debug($"[UrlHTTPHeaderLoader] {httpAttr.method}, {httpAttr.url}, {httpAttr.headername}, {httpAttr.headervalue} = {method}");
                ret.Add(httpAttr, method);
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

        public static void Merge(this Dictionary<HTTPAttribute, MethodInfo> keyValues, Assembly? assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));

            var attrib_methods = GetMethodAndAttribute<HTTPAttribute>(assembly);
            foreach (var attrib_method in attrib_methods)
            {
                if (attrib_method.attrib == null)
                    continue;
                if (attrib_method.method == null)
                    continue;
                if (!keyValues.TryAdd(attrib_method.attrib, attrib_method.method))
                {
                    Console.WriteLine($"Cannot merge {attrib_method.attrib.method} ,{attrib_method.attrib.url}.");
                }
                else
                    DebugPrinter.Debug($"[Merge HTTP] {attrib_method.attrib.method}, {attrib_method.attrib.url} = {attrib_method.method}");
            }
        }

        public static void Merge(this Dictionary<HTTPHeaderAttribute, MethodInfo> keyValues, Assembly? assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));

            var attrib_methods = GetMethodAndAttribute<HTTPHeaderAttribute>(assembly);
            foreach (var attrib_method in attrib_methods)
            {
                if (attrib_method.attrib == null)
                    continue;
                if (attrib_method.method == null)
                    continue;
                if (!keyValues.TryAdd(attrib_method.attrib, attrib_method.method))
                {
                    Console.WriteLine($"Cannot merge {attrib_method.attrib.method} ,{attrib_method.attrib.url}.");
                }
                else
                    DebugPrinter.Debug($"[Merge HTTPHeader] {attrib_method.attrib.method}, {attrib_method.attrib.url} = {attrib_method.method}");
            }
        }


        public static void Merge(this Dictionary<string, MethodInfo> keyValues, Assembly? assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));

            var attrib_methods = GetMethodAndAttribute<WSAttribute>(assembly);
            foreach (var attrib_method in attrib_methods)
            {
                if (attrib_method.attrib == null)
                    continue;
                if (attrib_method.method == null)
                    continue;
                if (!keyValues.TryAdd(attrib_method.attrib.url, attrib_method.method))
                {
                    Console.WriteLine($"Cannot merge {attrib_method.attrib.url}.");
                }
                else
                    DebugPrinter.Debug($"[Merge WS] {attrib_method.attrib.url} = {attrib_method.method}");
            }
        }

        public static void Override(this Dictionary<HTTPAttribute, MethodInfo> keyValues, Assembly? assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));

            var attrib_methods = GetMethodAndAttribute<HTTPAttribute>(assembly);
            foreach (var attrib_method in attrib_methods)
            {
                if (attrib_method.attrib == null)
                    continue;
                if (attrib_method.method == null)
                    continue;
                if (keyValues.ContainsKey(attrib_method.attrib))
                    keyValues.Remove(attrib_method.attrib);
                if (!keyValues.TryAdd(attrib_method.attrib, attrib_method.method))
                {
                    throw new Exception("KeyValues adding was not possible (Even after removed old data)");
                }
                else
                    DebugPrinter.Debug($"[Override HTTP] {attrib_method.attrib.method}, {attrib_method.attrib.url} = {attrib_method.method}");
            }
        }

        public static void Override(this Dictionary<HTTPHeaderAttribute, MethodInfo> keyValues, Assembly? assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));

            var attrib_methods = GetMethodAndAttribute<HTTPHeaderAttribute>(assembly);
            foreach (var attrib_method in attrib_methods)
            {
                if (attrib_method.attrib == null)
                    continue;
                if (attrib_method.method == null)
                    continue;
                if (keyValues.ContainsKey(attrib_method.attrib))
                    keyValues.Remove(attrib_method.attrib);
                if (!keyValues.TryAdd(attrib_method.attrib, attrib_method.method))
                {
                    throw new Exception("KeyValues adding was not possible (Even after removed old data)");
                }
                else
                    DebugPrinter.Debug($"[Override HTTPHeader] {attrib_method.attrib.method}, {attrib_method.attrib.url} = {attrib_method.method}");
            }
        }

        public static void Override(this Dictionary<string, MethodInfo> keyValues, Assembly? assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));

            var attrib_methods = GetMethodAndAttribute<WSAttribute>(assembly);
            foreach (var attrib_method in attrib_methods)
            {
                if (attrib_method.attrib == null)
                    continue;
                if (attrib_method.method == null)
                    continue;
                if (keyValues.ContainsKey(attrib_method.attrib.url))
                    keyValues.Remove(attrib_method.attrib.url);
                if (!keyValues.TryAdd(attrib_method.attrib.url, attrib_method.method))
                {
                    throw new Exception("KeyValues adding was not possible (Even after removed old data)");
                }
                else
                    DebugPrinter.Debug($"[Override WS] {attrib_method.attrib.url} = {attrib_method.method}");
            }
        }

        static List<(T attrib, MethodInfo method)> GetMethodAndAttribute<T>(Assembly? assembly) where T : Attribute
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));

            List<(T attrib, MethodInfo method)> values = new();

            var methods = assembly.GetTypes().SelectMany(x => x.GetMethods()).ToArray();
            //hacky  method to get custom attrib that is either void or bool (bool represent HTTP, void represent WS)
            var basemethods = methods.Where(x => x.GetCustomAttribute<T>() != null && (x.ReturnType == typeof(void) || x.ReturnType == typeof(bool))).ToArray();
            foreach (var method in basemethods)
            {
                if (method == null)
                    continue;
                var httpAttr = method.GetCustomAttribute<T>();
                if (httpAttr == null)
                    continue;
                values.Add((httpAttr, method));
            }
            return values;
        }
    }
}
