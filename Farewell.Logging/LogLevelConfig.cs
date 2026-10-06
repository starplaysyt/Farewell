namespace Farewell.Logging;

public struct LogLevelConfig(
    string label,
    ConsoleColor foregroundColor,
    ConsoleColor? backgroundColor,
    ConsoleColor messageColor)
{
    public string Label { get; set; } = label;
    public ConsoleColor Foreground { get; set; } = foregroundColor;
    public ConsoleColor? Background { get; set; } = backgroundColor;
    public ConsoleColor MessageColor { get; set; } = messageColor;
}