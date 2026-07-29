using System.Text.Json;
using System.Text.Json.Serialization;
using Vertical.Cli.Binding;
using Vertical.Cli.Invocation;
using Vertical.Cli.IO;

namespace Vertical.Cli.ScenarioTests.Common;

public class Handlers
{
    private static readonly JsonSerializerOptions _serializerOptions = new()
    {
        WriteIndented = true,
        Converters =
        {
            new JsonStringEnumConverter<CompressionType>(),
            new JsonStringEnumConverter<EncryptionType>()
        }
    };
    
    public class CreateHandler(IConsole console) : IHandler<ICreateCommandOptions>
    {
        /// <inheritdoc />
        public Task<int> HandleAsync([GeneratedBinding] ICreateCommandOptions options, CancellationToken cancellationToken)
        {
            var output = new
            {
                options.ComputeSha,
                options.IncludeMetadata,
                InputFiles = options.InputFiles.Select(file => file.Name),
                OutputFile = options.OutputFile.Name,
                options.OutputFileSplitSize,
                options.Overwrite,
                options.Properties,
                options.Timeout,
                options.CompressionType,
                options.EncryptionType
            };
            
            console.Out.WriteLine(JsonSerializer.Serialize(output, _serializerOptions));
            return Task.FromResult(0);
        }
    }

    public class ExtractHandler(IConsole console) : IHandler<IExtractCommandOptions>
    {
        /// <inheritdoc />
        public Task<int> HandleAsync([GeneratedBinding] IExtractCommandOptions options, CancellationToken cancellationToken)
        {
            var output = new
            {
                options.ComputeSha,
                InputFiles = options.InputFile.Name,
                options.Overwrite,
                options.Timeout,
                options.CompressionType,
                options.EncryptionType
            };
            
            console.Out.WriteLine(JsonSerializer.Serialize(output, _serializerOptions));
            return Task.FromResult(0);
        }
    }
}