namespace BasicDemo;

public interface IEncryptionOptions
{
    EncryptionType? EncryptionType { get; }
    string? Secret { get; }
}