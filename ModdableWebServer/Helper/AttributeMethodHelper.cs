using ModdableWebServer.Attributes;
using System.Reflection;

namespace ModdableWebServer.Helper;

public static class AttributeMethodHelper
{
    public static Dictionary<HTTPAttribute, MethodInfo> UrlHTTPLoader(Assembly? assembly)
    {
        if (assembly == null)
            return [];

        Dictionary<HTTPAttribute, MethodInfo> ret = [];
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
            return [];

        Dictionary<HTTPHeaderAttribute, MethodInfo> ret = [];
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
            return [];

        Dictionary<string, MethodInfo> ret = [];
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
        ArgumentNullException.ThrowIfNull(assembly);

        var attrib_methods = GetMethodAndAttribute<HTTPAttribute>(assembly);
        foreach (var (attrib, method) in attrib_methods)
        {
            if (attrib == null)
                continue;
            if (method == null)
                continue;
            if (!keyValues.TryAdd(attrib, method))
            {
                Console.WriteLine($"Cannot merge {attrib.method} ,{attrib.url}.");
            }
            else
                DebugPrinter.Debug($"[Merge HTTP] {attrib.method}, {attrib.url} = {method}");
        }
    }

    public static void Merge(this Dictionary<HTTPHeaderAttribute, MethodInfo> keyValues, Assembly? assembly)
    {
        ArgumentNullException.ThrowIfNull(assembly);

        var attrib_methods = GetMethodAndAttribute<HTTPHeaderAttribute>(assembly);
        foreach (var (attrib, method) in attrib_methods)
        {
            if (attrib == null)
                continue;
            if (method == null)
                continue;
            if (!keyValues.TryAdd(attrib, method))
            {
                Console.WriteLine($"Cannot merge {attrib.method} ,{attrib.url}.");
            }
            else
                DebugPrinter.Debug($"[Merge HTTPHeader] {attrib.method}, {attrib.url} = {method}");
        }
    }


    public static void Merge(this Dictionary<string, MethodInfo> keyValues, Assembly? assembly)
    {
        ArgumentNullException.ThrowIfNull(assembly);

        var attrib_methods = GetMethodAndAttribute<WSAttribute>(assembly);
        foreach (var (attrib, method) in attrib_methods)
        {
            if (attrib == null)
                continue;
            if (method == null)
                continue;
            if (!keyValues.TryAdd(attrib.url, method))
            {
                Console.WriteLine($"Cannot merge {attrib.url}.");
            }
            else
                DebugPrinter.Debug($"[Merge WS] {attrib.url} = {method}");
        }
    }

    public static void Override(this Dictionary<HTTPAttribute, MethodInfo> keyValues, Assembly? assembly)
    {
        ArgumentNullException.ThrowIfNull(assembly);

        var attrib_methods = GetMethodAndAttribute<HTTPAttribute>(assembly);
        foreach (var (attrib, method) in attrib_methods)
        {
            if (attrib == null)
                continue;
            if (method == null)
                continue;
            keyValues.Remove(attrib);
            if (!keyValues.TryAdd(attrib, method))
            {
                throw new Exception("KeyValues adding was not possible (Even after removed old data)");
            }
            else
                DebugPrinter.Debug($"[Override HTTP] {attrib.method}, {attrib.url} = {method}");
        }
    }

    public static void Override(this Dictionary<HTTPHeaderAttribute, MethodInfo> keyValues, Assembly? assembly)
    {
        ArgumentNullException.ThrowIfNull(assembly);

        var attrib_methods = GetMethodAndAttribute<HTTPHeaderAttribute>(assembly);
        foreach (var (attrib, method) in attrib_methods)
        {
            if (attrib == null)
                continue;
            if (method == null)
                continue;
            keyValues.Remove(attrib);
            if (!keyValues.TryAdd(attrib, method))
            {
                throw new Exception("KeyValues adding was not possible (Even after removed old data)");
            }
            else
                DebugPrinter.Debug($"[Override HTTPHeader] {attrib.method}, {attrib.url} = {method}");
        }
    }

    public static void Override(this Dictionary<string, MethodInfo> keyValues, Assembly? assembly)
    {
        ArgumentNullException.ThrowIfNull(assembly);

        var attrib_methods = GetMethodAndAttribute<WSAttribute>(assembly);
        foreach (var (attrib, method) in attrib_methods)
        {
            if (attrib == null)
                continue;
            if (method == null)
                continue;
            keyValues.Remove(attrib.url);
            if (!keyValues.TryAdd(attrib.url, method))
            {
                throw new Exception("KeyValues adding was not possible (Even after removed old data)");
            }
            else
                DebugPrinter.Debug($"[Override WS] {attrib.url} = {method}");
        }
    }

    static List<(T attrib, MethodInfo method)> GetMethodAndAttribute<T>(Assembly? assembly) where T : Attribute
    {
        ArgumentNullException.ThrowIfNull(assembly);

        List<(T attrib, MethodInfo method)> values = [];

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
