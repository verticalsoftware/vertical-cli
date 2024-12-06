using Microsoft.Extensions.Logging;
using Vertical.Cli.Conversion;

namespace CliDemo;

public class LogLevelConverter : ValueConverter<LogLevel>
{
    /// <inheritdoc />
    public override LogLevel Convert(string str)
    {
        return str.ToLower() switch
        {
            "m" => LogLevel.Warning,
            "minimal" => LogLevel.Warning,
            "n" => LogLevel.Information,
            "normal" => LogLevel.Information,
            "d" => LogLevel.Debug,
            "diagnostic" => LogLevel.Debug,
            _ => throw new ArgumentException("invalid log level")
        };
    }
}