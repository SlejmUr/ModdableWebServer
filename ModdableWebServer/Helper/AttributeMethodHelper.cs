using ModdableWebServer.Attributes;
using System.Reflection;

namespace ModdableWebServer.Helper;

public static class AttributeMethodHelper
{
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
                DebugPrinter.Debug($"[Merge HTTP] Cannot merge {attrib.method} ,{attrib.url}.");
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
                DebugPrinter.Debug($"[Merge HTTPHeader] Cannot merge {attrib.method} ,{attrib.url}.");
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
                DebugPrinter.Debug($"[Merge WS] Cannot merge {attrib.url}.");
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
                throw new Exception("KeyValues adding was not possible (Even after removed old data)");
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
                throw new Exception("KeyValues adding was not possible (Even after removed old data)");
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
                throw new Exception("KeyValues adding was not possible (Even after removed old data)");
            else
                DebugPrinter.Debug($"[Override WS] {attrib.url} = {method}");
        }
    }

    public static Dictionary<T, MethodInfo> GetMethodAndAttribute<T>(Assembly? assembly) where T : Attribute
    {
        ArgumentNullException.ThrowIfNull(assembly);

        Dictionary<T, MethodInfo> values = [];

        var methods = assembly.GetTypes().SelectMany(x => x.GetMethods()).ToArray();
        //hacky  method to get custom attrib that is either void or bool (bool represent HTTP, void represent WS)
        var basemethods = methods.Where(x => x.IsDefined(typeof(T)) && (x.ReturnType == typeof(void) || x.ReturnType == typeof(bool))).ToArray();
        foreach (var method in basemethods)
        {
            if (method == null)
                continue;
            var attribs = method.GetCustomAttributes<T>();
            if (attribs == null)
                continue;
            foreach (var attrib in attribs)
            {
                if (attrib == null) 
                    continue;
                values.Add(attrib, method);
            }
        }
        return values;
    }
}
