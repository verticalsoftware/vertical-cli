namespace BasicSetup;

public enum LogLevel
{
    Debug,
    Information,
    Warning,
    Error
}

public record Model(
    LogLevel? LogLevel,
    string[] Sources,
    DirectoryInfo? DirectoryInfo,
    FileInfo? FileInfo,
    Uri? BaseUrl,
    uint? Port,
    string? UserId);