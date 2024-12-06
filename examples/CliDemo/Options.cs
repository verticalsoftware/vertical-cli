using Microsoft.Extensions.Logging;

namespace CliDemo;

public enum CompressionType
{
    GZip,
    Deflate,
    Brotli
}

public enum EncryptionAlg
{
    None,
    Aes,
    Des,
    TripleDes,
    Rc2
}

public class CommonOptions
{
    public CompressionType CompressionType { get; set; }
    public EncryptionAlg EncryptionAlg { get; set; }
    public string? Cipher { get; set; }
    public LogLevel LogLevel { get; set; }
    public object ApplicationData { get; set; } = default!;
}

public class OperationOptions : CommonOptions
{
    public bool Overwrite { get; set; }
    public bool PrintSha { get; set; }
    public FileInfo Source { get; set; } = default!;
    public FileInfo Output { get; set; } = default!;
}