using Vertical.Cli.Binding;

namespace BasicDemo;

[GeneratedBinding]
public interface ICreateCommandOptions : ICompressionOptions, IEncryptionOptions
{
    DirectoryInfo[] ScanDirectories { get; }
    DirectoryInfo OutputDirectory { get; }
    string[] Patterns { get; }
    SplitSize SplitSize { get; }
    bool NoManifest { get; }
    Dictionary<string, string> Metadata { get; }
}