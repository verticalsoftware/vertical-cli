using Shouldly;
using Vertical.Cli.Configuration;
using Vertical.Cli.Configuration.Assertion;
using Vertical.Cli.Configuration.Assertion.Types;
using Vertical.Cli.Conversion;

namespace Vertical.Cli.ScenarioTests.ReproTests;

// https://github.com/verticalsoftware/vertical-cli/issues/3

public class UncaughtPropertyBindingAssertionTests
{
    public interface IColors
    {
        string Color { get; }
    }

    public interface IShapes
    {
        string Shape { get; }
    }

    public interface IOptions : IColors, IShapes
    {
    }

    public record Options(string Color = "", string Shape = "") : IOptions
    {
    }

    [Fact]
    public void UnmappedPropertyBinding_Caught_In_Assertions()
    {
        var command = new RootCommand("test");
        command.SetHandler<IOptions>((_, _) => Task.FromResult(0));
        
        var app = new CommandLineApplication(command);
        app.ConfigureModel<IOptions>(m => m.SetBinder(_ => new Options()));
        app.ConfigureParser<IShapes>(s => s.ParseOption(x => x.Shape));
        app.AddArgumentConverter(Converters.Default);

        app.GetConfigurationAssertions()
            .Single()
            .ShouldBeOfType<MissingPropertyBindingAssertion>();
    }
}