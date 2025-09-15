namespace ModdableWebServer.Helper;

public static class UrlHelper
{
    public static string ReasonFail { get; internal set; } = string.Empty;

    #region Parameter url stuff
    public static bool Match(string url, string pattern, out Dictionary<string, string> vals)
    {
        vals = [];
        ArgumentNullException.ThrowIfNull(url);
        ArgumentNullException.ThrowIfNull(pattern);

        string[] urlParts = SplitUrl(url);
        string[] patternParts = SplitUrl(pattern);

        int urlLen = urlParts.Count(static x => !x.Contains('='));
        int patternLen = patternParts.Count(static x => !x.Contains('=') && !x.Contains("!args"));

        if (urlLen != patternLen)
        {
            ReasonFail = $"Pattern len not same!\n {urlLen} != {patternLen}\nUrlParts:{string.Join(" ", urlParts)}\nPatternPaths: {string.Join(" ", patternParts)}";
            return false;
        }

        bool hasArg = pattern.EndsWith("{!args}");

        // Normal parameter parsing.
        for (int i = 0; i < urlParts.Length; i++)
        {
            string part = urlParts[i];
            if (hasArg && (patternParts.Length - 1) <= i && part.Contains('='))
            {
                var url_part = part.Split('=');
                vals.Add(url_part[0], url_part[1]);
                continue;
            }
            if (!ParseParameter(part, patternParts[i], ref vals))
            {
                ReasonFail = $"Parsing parameters did not work out {part} {patternParts[i]}";
                return false;
            }
        }
        ReasonFail = string.Empty;
        return true;
    }

    private static bool ParseParameter(string urlPart, string patternPart, ref Dictionary<string, string> vals)
    {
        string paramName = ExtractParameter(patternPart);
        if (!string.IsNullOrEmpty(paramName))
        {
            vals.Add(
            paramName.Replace("{", string.Empty).Replace("}", string.Empty),
                urlPart.Split('=').Last());
            return true;
        }
        if (!urlPart.Equals(patternPart))
        {
            vals = [];
            return false;
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
            return pattern.Substring(indexStart, indexEnd - indexStart + 1);

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
