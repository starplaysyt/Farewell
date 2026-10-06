using Farewell.Abstractions.Logging;

namespace Farewell.Logging;

public class LoggingConfiguration
{
    public static LoggingConfiguration Instance { get; } = new LoggingConfiguration();

    public LogLevelConfig TraceConfig =
        new("TRACE", ConsoleColor.DarkGray, null, ConsoleColor.DarkGray);

    public LogLevelConfig DebugConfig =
        new("DEBUG", ConsoleColor.White, ConsoleColor.DarkCyan, ConsoleColor.Gray);

    public LogLevelConfig InfoConfig =
        new("INFO", ConsoleColor.White, null, ConsoleColor.White);

    public LogLevelConfig WarnConfig =
        new("WARN", ConsoleColor.Black, ConsoleColor.Yellow, ConsoleColor.Yellow);

    public LogLevelConfig ErrorConfig =
        new("ERROR", ConsoleColor.White, ConsoleColor.DarkRed, ConsoleColor.Red);

    public LogLevelConfig FatalConfig =
        new("FATAL", ConsoleColor.White, ConsoleColor.DarkMagenta, ConsoleColor.Magenta);

    public LogLevelConfig DefaultConfig =
        new("???", ConsoleColor.White, null, ConsoleColor.White);

    public bool IsDebug { get; set; } = false;

    public bool ShowTimestamp { get; set; } = true;

    public bool ShowSenderName { get; set; } = true;

    public LogLevel MinimumLevel { get; set; } = LogLevel.Info;

    public LogLevelConfig GetLevelConfig(LogLevel level)
    {
        return level switch
        {
            LogLevel.Trace => TraceConfig,
            LogLevel.Debug => DebugConfig,
            LogLevel.Info => InfoConfig,
            LogLevel.Warn => WarnConfig,
            LogLevel.Error => ErrorConfig,
            LogLevel.Fatal => FatalConfig,
            _ => DefaultConfig
        };
    }

    // public static ConsoleColor InfoLabelColor = ConsoleColor.White;
    // public static ConsoleColor InfoSenderColor = ConsoleColor.Gray;
    // public static ConsoleColor InfoMessageColor = ConsoleColor.Gray;
    //     
    // public static ConsoleColor DoneLabelColor = ConsoleColor.Green;
    // public static ConsoleColor DoneSenderColor = ConsoleColor.DarkGreen;
    // public static ConsoleColor DoneMessageColor = ConsoleColor.DarkGreen;
    //     
    // public static ConsoleColor WarningLabelColor = ConsoleColor.Yellow;
    // public static ConsoleColor WarningSenderColor = ConsoleColor.White;
    // public static ConsoleColor WarningMessageColor = ConsoleColor.DarkYellow;
    //     
    // public static ConsoleColor ErrorLabelColor = ConsoleColor.Red;
    // public static ConsoleColor ErrorSenderColor = ConsoleColor.White;
    // public static ConsoleColor ErrorMessageColor = ConsoleColor.DarkRed;
    //     
    // public static ConsoleColor DebugLabelColor = ConsoleColor.Magenta;
    // public static ConsoleColor DebugSenderColor = ConsoleColor.DarkMagenta;
    // public static ConsoleColor DebugMessageColor = ConsoleColor.DarkMagenta;
    //     
    // public static ConsoleColor FatalLabelColor = ConsoleColor.Red;
    // public static ConsoleColor FatalSenderColor = ConsoleColor.DarkRed;
    // public static ConsoleColor FatalMessageColor = ConsoleColor.DarkRed;


}