namespace Vertical.Cli.ScenarioTests.Common;

public enum CompressionType
{
    GZip,
    Brotli
}

public enum EncryptionType
{
    AES,
    RSA
}

public interface ISharedOptions
{
    CompressionType CompressionType { get; }
    EncryptionType EncryptionType { get; }
    string SecretKey { get; }
    TimeSpan? Timeout { get; }
    bool ComputeSha { get; }
    bool Overwrite { get; }

}

public interface ICreateOptions
{
    FileInfo[] InputFiles { get; }
    FileInfo OutputFile { get; }
    FileSize OutputFileSplitSize { get; }
    bool IncludeMetadata { get; }
    Dictionary<string, string> Properties { get; }
}

public interface ICreateCommandOptions : ICreateOptions, ISharedOptions
{
}

public interface IExtractOptions
{
    FileInfo InputFile { get; }
    DirectoryInfo OutputPath { get; }
}

public interface IExtractCommandOptions : IExtractOptions, ISharedOptions
{
}

