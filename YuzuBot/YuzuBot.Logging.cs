using Discord;
using System.Diagnostics;

namespace YuzuBot;
internal partial class YuzuBot
{
    private Task OnLog(LogMessage msg)
    {
        Console.WriteLine(msg.ToString());
        return Task.CompletedTask;
    }

    private static void Log(string msg, LogSeverity severity, Exception? exception = null)
    {
        var log = new LogMessage(severity, "YuzuBot", msg, exception);
        Console.WriteLine(log.ToString());
    }

#pragma warning disable IDE0051 // Remove unused private members
    private static void LogInfo(string msg, Exception? exception = null) => Log(msg, LogSeverity.Info, exception);
    private static void LogWarning(string msg, Exception? exception = null) => Log(msg, LogSeverity.Warning, exception);
    private static void LogError(string msg, Exception? exception = null) => Log(msg, LogSeverity.Error, exception);
    private static void LogCritical(string msg, Exception? exception = null) => Log(msg, LogSeverity.Critical, exception);

    [Conditional("DEBUG")]
    private static void LogDebug(string msg, Exception? exception = null) => Log(msg, LogSeverity.Debug, exception);

    [Conditional("DEBUG")]
    private static void LogVerbose(string msg, Exception? exception = null) => Log(msg, LogSeverity.Verbose, exception);
#pragma warning restore IDE0051 // Remove unused private members
}
