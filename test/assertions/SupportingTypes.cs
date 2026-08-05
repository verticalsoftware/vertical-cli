using Vertical.Cli.Invocation;

namespace Vertical.Cli.ConfigurationAssertionTests;

public interface IModel
{
    string UserId { get; }
    bool Unbound { get; }
    string MultipleBindings { get; }
    string Password { get; }
    int Port { get; }
    int[] Variadic1 { get; }
    int[] Variadic2 { get; }
}

public sealed class ModelHandler : IHandler<IModel>
{
    /// <inheritdoc />
    public Task<int> HandleAsync(IModel options, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}