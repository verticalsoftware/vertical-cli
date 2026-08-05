using Vertical.Cli.Configuration;
using Vertical.Cli.Configuration.Assertion;
using Vertical.Cli.Configuration.Assertion.Types;

namespace Vertical.Cli.ConfigurationAssertionTests;

public class AssertionTests
{
    [Fact]
    public Task AllErrors_Reports_Verified_Output()
    {
        var rootCommand = new RootCommand("app");
        var sub1 = new SubCommand("create");
        rootCommand.AddSubCommand(sub1);

        var sub2 = new SubCommand("create");
        sub2.SetHandler<IModel, ModelHandler>();
        rootCommand.AddSubCommand(sub2);

        var app = new CommandLineApplication(rootCommand);

        app.ConfigureParser<IModel>(model => model
            .ParseOption(x => x.MultipleBindings)
            .MapStaticValue(x => x.MultipleBindings, "value")
            .ParseOption(x => x.Password, ["-p", "--password"])
            .ParseOption(x => x.Port, ["-p", "--port"])
            .ParseRepeatableArgument(x => x.Variadic1, ordinalPosition: 0, Arity.ZeroOrMore)
            .ParseRepeatableArgument(x => x.Variadic2, ordinalPosition: 0, Arity.ZeroOrMore)
        );

        var assertions = app.GetConfigurationAssertions();
        
        return Verify(ConfigurationAssertion.GetAssertionsAsText(assertions));
    }
}