using Vertical.Cli.Invocation;

namespace BasicDemo;

public sealed class CompressHandler : IHandler<IOptions>
{
    /// <inheritdoc />
    public Task<int> HandleAsync(IOptions options, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Compressing {options.SourceFiles.Length} files...");

        return Task.FromResult(0);
    }
}