using Vertical.Cli.Binding;

namespace Vertical.Archiver;

public enum CompressionType
{
    GZip,
    Deflate
}

public enum EncryptionType
{
    None,
    Aes,
    Rsa
}

public enum Checksum
{
    None,
    Md5,
    Sha1,
    Sha256
}

public interface IArchivingOptions
{
    CompressionType Compression { get; }
    
    EncryptionType Encryption { get; }
    
    TimeSpan? Timeout { get; }
    
    Checksum Checksum { get; }
    
    FileInfo? KeyFile { get; }
    
    string? Passphrase { get; }
}

public interface ICreateOptions
{
    FileSystemInfo[] Sources { get; }
    
    FileInfo OutputPath { get; }
}

public interface IExtractOptions
{
    FileInfo SourcePath { get; }
    
    DirectoryInfo OutputPath { get; }
}

[GeneratedBinding]
public record CreateOptions(
    CompressionType Compression,
    EncryptionType Encryption,
    TimeSpan? Timeout,
    Checksum Checksum,
    FileInfo? KeyFile,
    string? Passphrase,
    FileSystemInfo[] Sources,
    FileInfo OutputPath) : IArchivingOptions, ICreateOptions;

public record ExtractOptions(
    CompressionType Compression,
    EncryptionType Encryption,
    TimeSpan? Timeout,
    Checksum Checksum,
    FileInfo? KeyFile,
    string? Passphrase,
    FileInfo SourcePath,
    DirectoryInfo OutputPath) : IArchivingOptions, IExtractOptions;


