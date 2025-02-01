using LLibrary;

namespace ModdableWebServer.Helper;

public class DebugPrinter
{
    public static bool PrintToConsole = false;
    public static bool EnableLogs = false;
    public static L logger = new(true, directory: "mws_logs");

    public static void Debug(string ToPrint, string prefix = "DEBUG")
    {
        Print(ToPrint, prefix);
    }

    static void Print(string ToPrint, string prefix)
    {
        if (PrintToConsole)
        {
            Console.WriteLine($"[{prefix}] {ToPrint}");
        }
        if (EnableLogs)
        {
            logger.Log(prefix, ToPrint == null ? "NULL" : ToPrint);
        }
    }
}
