using Farewell.Abstractions.Logging;

namespace Farewell.Logging;

public class ConsoleLogger : ILogger
{
    public LoggingConfiguration Config { get; set; } = LoggingConfiguration.Instance;
    
    public void Log(LogLevel level, string message, string sender, Exception? exception = null)
    {
        var config = Config;
        if (level < config.MinimumLevel)
            return;

        var configLevel = Config.GetLevelConfig(level);
        var originalFg = Console.ForegroundColor;
        var originalBg = Console.BackgroundColor;
        
        try
        {
            if (config.ShowTimestamp)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] ");
            }

            Console.ForegroundColor = configLevel.Foreground;
            if (configLevel.Background.HasValue)
                Console.BackgroundColor = configLevel.Background.Value;

            Console.Write($" {configLevel.Label,-5} ");
            Console.BackgroundColor = originalBg;

            if (config.ShowSenderName)
            {
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.Write($" [ " + sender + " ] ");
            }

            Console.ForegroundColor = configLevel.MessageColor;
            Console.WriteLine(message);

            if (exception != null)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine($"  └─ {exception.GetType().Name}: {exception.Message}");

                if (exception.StackTrace != null)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    foreach (var line in exception.StackTrace.Split('\n'))
                    {
                        Console.WriteLine($"     {line.Trim()}");
                    }
                }
            }
        }
        finally
        {
            Console.ForegroundColor = originalFg;
            Console.BackgroundColor = originalBg;
        }
    }
}