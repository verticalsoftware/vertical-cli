using Vertical.Cli.Invocation;

namespace BasicDemo;

public sealed class CreateHandler : IHandler<ICreateCommandOptions>
{
    /// <inheritdoc />
    public Task<int> HandleAsync(ICreateCommandOptions options, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}