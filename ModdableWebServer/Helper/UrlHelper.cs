namespace ModdableWebServer.Helper;

public class UrlHelper
{
    #region Parameter url stuff
    public static bool Match(string url, string pattern, out Dictionary<string, string> vals)
    {
        vals = [];
        ArgumentNullException.ThrowIfNull(url);
        ArgumentNullException.ThrowIfNull(pattern);

        string[] urlParts = SplitUrl(url);
        string[] patternParts = SplitUrl(pattern);

        if (urlParts.Length != patternParts.Length)
            return false;

        DebugPrinter.Debug("URL Parts: " + string.Join(" ", urlParts));
        DebugPrinter.Debug("Pattern Paths: " + string.Join(" ", patternParts));

        if (patternParts.Length >= 2 && 
            pattern.Contains('?') && 
            pattern.Split("?")[1] == "{args}")
        {
            for (int i = 0; i < urlParts.Length; i++)
            {
                if ((patternParts.Length - 1) <= i)
                {
                    var url_part = urlParts[i].Split('=');
                    vals.Add(url_part[0], url_part[1]);
                    continue;
                }
                string paramName = ExtractParameter(patternParts[i]);

                if (string.IsNullOrEmpty(paramName))
                {
                    // no pattern
                    if (!urlParts[i].Equals(patternParts[i]))
                    {
                        vals = [];
                        return false;
                    }
                }
                else
                {
                    vals.Add(
                        paramName.Replace("{", "").Replace("}", ""),
                        urlParts[i].Split('=').Last());
                }
            }
            return true;
        }

        for (int i = 0; i < urlParts.Length; i++)
        {
            string paramName = ExtractParameter(patternParts[i]);
            if (string.IsNullOrEmpty(paramName))
            {
                // no pattern
                if (!urlParts[i].Equals(patternParts[i]))
                {
                    vals = [];
                    return false;
                }
            }
            else
            {
                vals.Add(
                    paramName.Replace("{", "").Replace("}", ""),
                    urlParts[i].Split('=').Last());
            }
        }
        return true;
    }

    private static string ExtractParameter(string pattern)
    {
        ArgumentNullException.ThrowIfNull(pattern);

        if (!pattern.Contains('{'))
            return string.Empty;

        if (!pattern.Contains('}'))
            return string.Empty;

        int indexStart = pattern.IndexOf('{');
        int indexEnd = pattern.LastIndexOf('}');
        if (indexEnd - 1 > indexStart)
        {
            return pattern.Substring(indexStart, indexEnd - indexStart + 1);
        }
        return string.Empty;
    }

    private static string[] SplitUrl(string url)
    {
        string[] urlPaths = url.Split('/', StringSplitOptions.RemoveEmptyEntries);
        urlPaths = SplitUrl(urlPaths, '?');
        urlPaths = SplitUrl(urlPaths, '&');
        return urlPaths;
    }

    private static string[] SplitUrl(string[] urlParts, char splitChar)
    {
        List<string> parts = [];

        foreach (var part in urlParts)
        {
            if (!part.Contains(splitChar))
            {
                parts.Add(part);
                continue;
            }

            string[] subParts = part.Split(splitChar, StringSplitOptions.RemoveEmptyEntries);
            parts.AddRange(subParts);
        }

        return [.. parts];
    }

    #endregion
}
