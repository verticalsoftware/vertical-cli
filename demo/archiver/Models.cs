using Vertical.Cli.Binding;

namespace Vertical.Archiver;

public enum CompressionType
{
    GZip,
    Deflate
}

public enum CheckSumType
{
    Md5,
    Sha1,
    Sha256
}

public interface IChecksumOptions
{
    CheckSumType? CheckSum { get; }
}

[GeneratedBinding]
public interface IArchivingOptions
{
    CompressionType Compression { get; }
    FileSystemInfo SourcePath { get; }
    string? SourcePattern { get; }
    FileInfo? OutputPath { get; }
}

public interface IEncryptionOptions
{
    string? Passphrase { get; }
}

[GeneratedBinding]
public class ArchivingOptions
{
    public ArchivingOptions(string param)
    {
        
    }
    
    public required CompressionType Compression { get; set; }
    public required FileSystemInfo SourcePath { get; set; }
    public required string? SourcePattern { get; set; }
    public required FileInfo? OutputPath { get; set; }
}

[GeneratedBinding]
public record ArchivingOptionsRecord(CompressionType Compression, FileSystemInfo SourcePath);