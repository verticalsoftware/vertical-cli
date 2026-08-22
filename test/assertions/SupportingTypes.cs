using Vertical.Cli.IO;

namespace Vertical.Cli.ConfigurationAssertionTests;

public interface IModel
{
    string Option { get; }
    string[] Options { get; }
    string[] Options2 { get; }
}

public interface IModelWithPrivateBinding
{
    OutputWriter OutputWriter { get; }
}