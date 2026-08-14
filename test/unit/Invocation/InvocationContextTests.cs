using Shouldly;
using Vertical.Cli.Configuration;
using Vertical.Cli.Diagnostics;
using Vertical.Cli.Invocation;

namespace Vertical.Cli.UnitTests.Invocation;

public class InvocationContextTests
{
    public sealed class Error : CommandLineError
    {
        /// <inheritdoc />
        public Error() : base("Error")
        {
        }
    }
    
    [Fact]
    public void Assert_State_Throws_With_Errors()
    {
        var configuration = new RootConfiguration(new RootCommand("test"));
        var unit = new InvocationContext(configuration, []);
        unit.AddError(new Error());

        Should.Throw<CommandLineException>(unit.AssertState);
    }
    
    [Fact]
    public void Assert_State_Does_Not_Throw_With_Errors()
    {
        var configuration = new RootConfiguration(new RootCommand("test"));
        var unit = new InvocationContext(configuration, []);

        Should.NotThrow(unit.AssertState);
    }

    [Fact]
    public void RequestCancel_Invokes_CancellationTokenSource()
    {
        var configuration = new RootConfiguration(new RootCommand("test"));
        var unit = new InvocationContext(configuration, []);
        unit.RequestCancel();
        unit.CancellationToken.IsCancellationRequested.ShouldBeTrue();
    }
}