namespace Vertical.Cli;

/// <summary>
/// Throws when the application is misconfigured.
/// </summary>
/// <param name="message">A message that describes the exception</param>
/// <param name="innerException">The exception that caused this occurrence</param>
public sealed class CliConfigurationException(string message, Exception? innerException = null)
    : Exception(message, innerException)
{
    
}