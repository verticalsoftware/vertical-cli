using System.Collections.Immutable;
using Vertical.Cli.Binding;

namespace BasicDemo;

public enum CompressionAlgorithm
{
    GZip,
    Brotli
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

// public record Options(
//     CompressionAlgorithm CompressionType,
//     FileInfo[] SourceFiles,
//     FileInfo OutputFile, 
//     bool PrintSha,
//     int SplitSizeKb,
//     TimeSpan? Timeout) : IOptions
// {
// }