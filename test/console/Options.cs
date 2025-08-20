namespace Vertical.Cli.ConsoleTests;

public enum ChecksumType
{
    Md5,
    Sha1,
    Sha256
}

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

public interface IArchivingOptions
{
    CompressionType Compression { get; }
    
    EncryptionType Encryption { get; }
    
    public string? Secret { get; }
    
    public TimeSpan? Timeout { get; }
}

public interface ICreateOptions
{
    FileSystemInfo[] Sources { get; }
    
    FileInfo OutputFile { get; }
}

public interface IExtractOptions
{
    FileInfo Source { get; }
    
    DirectoryInfo OutputPath { get; }
}