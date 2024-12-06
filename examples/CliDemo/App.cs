using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;
using Vertical.SpectreLogger;

namespace CliDemo;

public static class App
{
    public static async Task<int> CreateAsync(OperationOptions options, CancellationToken cancellationToken)
    {
        return await new Runtime(options.LogLevel).RunCreateAsync(options, cancellationToken);
    }

    public static async Task<int> ExtractAsync(OperationOptions options, CancellationToken cancellationToken)
    {
        return await new Runtime(options.LogLevel).RunExtractAsync(options, cancellationToken);
    }
    
    private sealed class Runtime(LogLevel logLevel)
    {
        private readonly ILogger logger = LoggerFactory.Create(builder => builder
                .ClearProviders()
                .SetMinimumLevel(logLevel)
                .AddSpectreConsole(spectre => spectre.SetMinimumLevel(logLevel)))
            .CreateLogger<Runtime>();

        public async Task<int> RunCreateAsync(OperationOptions options, CancellationToken cancellationToken)
        {
            await using var inputStream = File.OpenRead(options.Source.FullName);
            await using var memoryStream = new MemoryStream();
            await using var compressionStream = GetCompressionStream(
                options.CompressionType, 
                memoryStream, 
                CompressionMode.Compress);

            await inputStream.CopyToAsync(compressionStream, cancellationToken);
            await compressionStream.FlushAsync(cancellationToken);
            await WriteOutputFileAsync(memoryStream, options, cancellationToken);
            
            return 0;
        }

        public async Task<int> RunExtractAsync(OperationOptions options, CancellationToken cancellationToken)
        {
            await using var inputStream = await GetInputStreamAsync(options, cancellationToken);
            await using var outputStream = File.OpenWrite(options.Output.FullName);
            await using var compressionStream = GetCompressionStream(options.CompressionType,
                inputStream,
                CompressionMode.Decompress);

            logger.LogDebug("Decompressing using {inflater}", compressionStream.GetType());
            
            await compressionStream.CopyToAsync(outputStream, cancellationToken);
            await compressionStream.FlushAsync(cancellationToken);
            await outputStream.FlushAsync(cancellationToken);
            
            logger.LogInformation("Wrote decompressed file {path} (length={len}b)", options.Output,
                options.Output.Length);
            
            return 0;
        }

        private async Task<Stream> GetInputStreamAsync(OperationOptions options, CancellationToken cancellationToken)
        {
            if (options.EncryptionAlg == EncryptionAlg.None)
            {
                return File.OpenRead(options.Source.FullName);
            }

            logger.LogDebug("Decrypting contents...");
            
            await using var fileStream = File.OpenRead(options.Source.FullName);
            using var algorithm = GetEncryptionAlgorithm(options);
            var ivLength = algorithm.IV.Length;
            var iv = new byte[ivLength];

            if (await fileStream.ReadAsync(iv.AsMemory(), cancellationToken) != ivLength)
                throw new InvalidOperationException();

            await using var cryptoStream = new CryptoStream(fileStream,
                algorithm.CreateDecryptor(algorithm.Key, iv),
                CryptoStreamMode.Read);

            var memoryStream = new MemoryStream();
            await cryptoStream.CopyToAsync(memoryStream, cancellationToken);

            LogSha(memoryStream, "Compressed data");
            
            return memoryStream;
        }

        private async Task WriteOutputFileAsync(MemoryStream memoryStream, 
            OperationOptions options,
            CancellationToken cancellationToken)
        {
            LogSha(memoryStream, "Compressed data");
            
            if (options.EncryptionAlg == EncryptionAlg.None)
            {
                await File.WriteAllBytesAsync(options.Output.FullName,
                    memoryStream.ToArray().AsMemory(), 
                    cancellationToken);
                logger.LogInformation("Wrote unencrypted file {path}", options.Output);
                return;
            }

            logger.LogDebug("Encrypting source contents...");
            
            await using var fileStream = File.OpenWrite(options.Output.FullName);
            using var algorithm = GetEncryptionAlgorithm(options);
            await fileStream.WriteAsync(algorithm.IV.AsMemory(), cancellationToken);
            
            await using var cryptoStream = new CryptoStream(fileStream, 
                algorithm.CreateEncryptor(), 
                CryptoStreamMode.Write);

            memoryStream.Position = 0;
            
            await memoryStream.CopyToAsync(cryptoStream, cancellationToken);
            await cryptoStream.FlushAsync(cancellationToken);
            await fileStream.FlushAsync(cancellationToken);
            
            logger.LogInformation("Wrote encrypted ({alg}) file {path}",
                options.EncryptionAlg,
                options.Output);
        }

        private void LogSha(MemoryStream stream, string type)
        {
            stream.Position = 0;
            var hash = Convert.ToHexString(SHA256.HashData(stream)).ToLower();
            logger.LogDebug("{type} hash={hash} (length={len}b)", type, hash, stream.Length);
            stream.Position = 0;
        }

        private static SymmetricAlgorithm GetEncryptionAlgorithm(OperationOptions options)
        {
            SymmetricAlgorithm algorithm = options.EncryptionAlg switch
            {
                EncryptionAlg.Aes => Aes.Create(),
                EncryptionAlg.Des => DES.Create(),
                EncryptionAlg.Rc2 => RC2.Create(),
                _ => TripleDES.Create()
            };
        
            var bytes = Encoding.UTF8.GetBytes(options.Cipher!);
            var key = new byte[128/8];

            for (var c = 0; c < key.Length && c < bytes.Length; c++)
            {
                key[c] = bytes[c];
            }

            algorithm.Key = key;
            return algorithm;
        }

        private static Stream GetCompressionStream(CompressionType type, Stream stream, CompressionMode mode) => type switch
        {
            CompressionType.GZip => new GZipStream(stream, mode),
            CompressionType.Brotli => new BrotliStream(stream, mode),
            _ => new DeflateStream(stream, mode)
        };
    }
}