using System.Reflection;

namespace ModdableWebServer.Helper;

public static class AttributeMethodHelper
{
    public static void Merge<T>(this Dictionary<T, MethodInfo> keyValues, Assembly? assembly) where T : Attribute
    {
        ArgumentNullException.ThrowIfNull(assembly);

        var attrib_methods = GetMethodAndAttribute<T>(assembly);
        foreach (var (attrib, method) in attrib_methods)
        {
            if (attrib == null)
                continue;
            if (method == null)
                continue;
            if (!keyValues.TryAdd(attrib, method))
                Log.Warning($"Cannot merge {attrib}.");
            else
                Log.Verbose($"{attrib} = {method}");
        }
    }

    public static void Override<T>(this Dictionary<T, MethodInfo> keyValues, Assembly? assembly) where T : Attribute
    {
        ArgumentNullException.ThrowIfNull(assembly);

        var attrib_methods = GetMethodAndAttribute<T>(assembly);
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
                Log.Verbose($"[Override] {attrib} = {method}");
        }
    }

    public static Dictionary<T, MethodInfo> GetMethodAndAttribute<T>(Assembly? assembly) where T : Attribute
    {
        ArgumentNullException.ThrowIfNull(assembly);

        Dictionary<T, MethodInfo> values = [];

        foreach (var method in assembly.GetTypes().SelectMany(select =>
            select.GetMethods().Where(method => 
                method.IsDefined(typeof(T)) && 
                (method.ReturnType == typeof(void) || method.ReturnType == typeof(bool)
                )
            )
        ))
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
