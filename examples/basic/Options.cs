using System.Collections.Immutable;
using Vertical.Cli.Binding;

namespace BasicDemo;

public enum CompressionAlgorithm
{
    GZip,
    Brotli
}

public enum LogSeverity
{
    Debug,
    Normal,
    Minimal
}

[GeneratedBinding]
public interface IOptions
{
    CompressionAlgorithm CompressionType { get; }
    FileInfo[] SourceFiles { get; }
    FileInfo OutputFile { get; }
    bool PrintSha { get; }
    int SplitSizeKb { get; }
    TimeSpan? Timeout { get; }
}

public sealed class AppOptions
{
    public LogSeverity LogSeverity { get; set; }
}