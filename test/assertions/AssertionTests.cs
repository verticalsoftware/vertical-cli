using NSubstitute;
using Shouldly;
using Vertical.Cli.Configuration;
using Vertical.Cli.Configuration.Assertion;
using Vertical.Cli.Configuration.Assertion.Types;
using Vertical.Cli.Conversion;

namespace Vertical.Cli.ConfigurationAssertionTests;

public class AssertionTests
{
    [Fact]
    public void Errors_Not_Reported_With_Clean_Configuration()
    {
        var root = new RootCommand("app");
        var subCommand = new SubCommand("create");
        subCommand.SetHandler<IModel>((_,_) => Task.FromResult(0));
        root.AddSubCommand(subCommand);

        var subCommand2 = new SubCommand("remove");
        subCommand2.SetHandler<IModel>((_,_) => Task.FromResult(0));
        root.AddSubCommand(subCommand2);                    

        var app = new CommandLineApplication(root);
        app.ConfigureMiddleware(middleware => middleware
            .AddSwitch(
                "Version",
                "--version",
                _ => Task.FromResult<int?>(0)));
        app.ConfigureParser<IModel>(parser => parser
            .ParseOption(x => x.Option)
            .ParseRepeatableArgument(x => x.Options, ordinalPosition: 0, arity: Arity.One)
            .ParseRepeatableArgument(x => x.Options2, ordinalPosition: 1,  arity: Arity.One)
            .SetBinder(_ => Substitute.For<IModel>()));
        app.AddArgumentConverter(Converters.Default);
        app.AddCollectionConverter<string, string[]>(values => values.ToArray());

        app.GetConfigurationAssertions().ShouldBeEmpty();
    }
    
    [Fact]
    public Task Reports_ArgumentOrdinalPositionAssertion()
    {
        var root = new RootCommand("app");
        var subCommand = new SubCommand("create");
        subCommand.SetHandler<IModel>((_,_) => Task.FromResult(0));
        root.AddSubCommand(subCommand);  

        var app = new CommandLineApplication(root);
        app.ConfigureMiddleware(middleware => middleware
            .AddSwitch(
                "Version",
                "--version",
                _ => Task.FromResult<int?>(0)));
        app.ConfigureParser<IModel>(parser => parser
            .ParseOption(x => x.Option)
            .ParseRepeatableArgument(x => x.Options, ordinalPosition: 0, arity: Arity.One)
            .ParseRepeatableArgument(x => x.Options2, ordinalPosition: 0,  arity: Arity.One)
            .SetBinder(_ => Substitute.For<IModel>()));
        app.AddArgumentConverter(Converters.Default);
        app.AddCollectionConverter<string, string[]>(values => values.ToArray());

        return Verify(ConfigurationAssertion.GetAssertionsAsText(app.GetConfigurationAssertions()));
    }
    
    [Fact]
    public Task Reports_DeadEndCommandAssertion()
    {
        var root = new RootCommand("app");
        var subCommand = new SubCommand("create");
        root.AddSubCommand(subCommand);  

        var app = new CommandLineApplication(root);
        app.ConfigureMiddleware(middleware => middleware
            .AddSwitch(
                "Version",
                "--version",
                _ => Task.FromResult<int?>(0)));
        app.ConfigureParser<IModel>(parser => parser
            .ParseOption(x => x.Option)
            .ParseRepeatableArgument(x => x.Options, ordinalPosition: 0, arity: Arity.One)
            .ParseRepeatableArgument(x => x.Options2, ordinalPosition: 0,  arity: Arity.One)
            .SetBinder(_ => Substitute.For<IModel>()));
        app.AddArgumentConverter(Converters.Default);
        app.AddCollectionConverter<string, string[]>(values => values.ToArray());

        return Verify(ConfigurationAssertion.GetAssertionsAsText(app.GetConfigurationAssertions()));
    }
    
    [Fact]
    public Task Reports_DuplicateCommandNameAssertion()
    {
        var root = new RootCommand("app");
        var subCommand = new SubCommand("create");
        subCommand.SetHandler<IModel>((_,_) => Task.FromResult(0));
        root.AddSubCommand(subCommand);

        var subCommand2 = new SubCommand("create");
        subCommand2.SetHandler<IModel>((_,_) => Task.FromResult(0));
        root.AddSubCommand(subCommand2);                    

        var app = new CommandLineApplication(root);
        app.ConfigureMiddleware(middleware => middleware
            .AddSwitch(
                "Version",
                "--version",
                _ => Task.FromResult<int?>(0)));
        app.ConfigureParser<IModel>(parser => parser
            .ParseOption(x => x.Option)
            .ParseRepeatableArgument(x => x.Options, ordinalPosition: 0, arity: Arity.One)
            .ParseRepeatableArgument(x => x.Options2, ordinalPosition: 1,  arity: Arity.One)
            .SetBinder(_ => Substitute.For<IModel>()));
        app.AddArgumentConverter(Converters.Default);
        app.AddCollectionConverter<string, string[]>(values => values.ToArray());

        return Verify(ConfigurationAssertion.GetAssertionsAsText(app.GetConfigurationAssertions()));
    }
    
    [Fact]
    public Task Reports_DuplicatePropertyBindingAssertion()
    {
        var root = new RootCommand("app");
        var subCommand = new SubCommand("create");
        subCommand.SetHandler<IModel>((_,_) => Task.FromResult(0));
        root.AddSubCommand(subCommand);            

        var app = new CommandLineApplication(root);
        app.ConfigureParser<IModel>(parser => parser
            .ParseOption(x => x.Option, "--option1")
            .ParseOption(x => x.Option, "--option2")
            .ParseRepeatableArgument(x => x.Options, ordinalPosition: 0, arity: Arity.One)
            .ParseRepeatableArgument(x => x.Options2, ordinalPosition: 1,  arity: Arity.One)
            .SetBinder(_ => Substitute.For<IModel>()));
        app.AddArgumentConverter(Converters.Default);
        app.AddCollectionConverter<string, string[]>(values => values.ToArray());

        return Verify(ConfigurationAssertion.GetAssertionsAsText(app.GetConfigurationAssertions()));
    }
    
    [Fact]
    public Task Reports_MissingArgumentConverterAssertion()
    {
        var root = new RootCommand("app");
        var subCommand = new SubCommand("create");
        subCommand.SetHandler<IModel>((_,_) => Task.FromResult(0));
        root.AddSubCommand(subCommand);            

        var app = new CommandLineApplication(root);
        app.ConfigureParser<IModel>(parser => parser
            .ParseOption(x => x.Option)
            .ParseRepeatableArgument(x => x.Options, ordinalPosition: 0, arity: Arity.One)
            .ParseRepeatableArgument(x => x.Options2, ordinalPosition: 1,  arity: Arity.One)
            .SetBinder(_ => Substitute.For<IModel>()));
        app.AddCollectionConverter<string, string[]>(values => values.ToArray());

        return Verify(ConfigurationAssertion.GetAssertionsAsText(app.GetConfigurationAssertions()));
    }
    
    [Fact]
    public Task Reports_MissingCollectionConverterAssertion()
    {
        var root = new RootCommand("app");
        var subCommand = new SubCommand("create");
        subCommand.SetHandler<IModel>((_,_) => Task.FromResult(0));
        root.AddSubCommand(subCommand);            

        var app = new CommandLineApplication(root);
        app.ConfigureParser<IModel>(parser => parser
            .ParseOption(x => x.Option)
            .ParseRepeatableArgument(x => x.Options, ordinalPosition: 0, arity: Arity.One)
            .ParseRepeatableArgument(x => x.Options2, ordinalPosition: 1,  arity: Arity.One)
            .SetBinder(_ => Substitute.For<IModel>()));
        app.AddArgumentConverter(Converters.Default);

        return Verify(ConfigurationAssertion.GetAssertionsAsText(app.GetConfigurationAssertions()));
    }
    
    [Fact]
    public Task Reports_MissingModelBindingAssertion()
    {
        var root = new RootCommand("app");
        var subCommand = new SubCommand("create");
        subCommand.SetHandler<IModel>((_,_) => Task.FromResult(0));
        root.AddSubCommand(subCommand);                 

        var app = new CommandLineApplication(root);
        app.ConfigureMiddleware(middleware => middleware
            .AddSwitch(
                "Version",
                "--version",
                _ => Task.FromResult<int?>(0)));
        app.ConfigureParser<IModel>(parser => parser
            .ParseOption(x => x.Option)
            .ParseRepeatableArgument(x => x.Options, ordinalPosition: 0, arity: Arity.One)
            .ParseRepeatableArgument(x => x.Options2, ordinalPosition: 1, arity: Arity.One));
        app.AddArgumentConverter(Converters.Default);
        app.AddCollectionConverter<string, string[]>(values => values.ToArray());

        return Verify(ConfigurationAssertion.GetAssertionsAsText(app.GetConfigurationAssertions()));
    }
    
    [Fact]
    public Task Reports_MissingPropertyBindingAssertion()
    {
        var root = new RootCommand("app");
        var subCommand = new SubCommand("create");
        subCommand.SetHandler<IModel>((_,_) => Task.FromResult(0));
        root.AddSubCommand(subCommand);
        
        var app = new CommandLineApplication(root);
        app.ConfigureMiddleware(middleware => middleware
            .AddSwitch(
                "Version",
                "--version",
                _ => Task.FromResult<int?>(0)));
        app.ConfigureParser<IModel>(parser => parser
            .ParseOption(x => x.Option)
            .ParseRepeatableArgument(x => x.Options2, ordinalPosition: 1,  arity: Arity.One)
            .SetBinder(_ => Substitute.For<IModel>()));
        app.AddArgumentConverter(Converters.Default);
        app.AddCollectionConverter<string, string[]>(values => values.ToArray());

        return Verify(ConfigurationAssertion.GetAssertionsAsText(app.GetConfigurationAssertions()));
    }
    
    [Fact]
    public Task Reports_MultipleVariadicArgumentsAssertion()
    {
        var root = new RootCommand("app");
        var subCommand = new SubCommand("create");
        subCommand.SetHandler<IModel>((_,_) => Task.FromResult(0));
        root.AddSubCommand(subCommand); 

        var app = new CommandLineApplication(root);
        app.ConfigureMiddleware(middleware => middleware
            .AddSwitch(
                "Version",
                "--version",
                _ => Task.FromResult<int?>(0)));
        app.ConfigureParser<IModel>(parser => parser
            .ParseOption(x => x.Option)
            .ParseRepeatableArgument(x => x.Options, ordinalPosition: 0, arity: Arity.OneOrMore)
            .ParseRepeatableArgument(x => x.Options2, ordinalPosition: 1,  arity: Arity.One)
            .SetBinder(_ => Substitute.For<IModel>()));
        app.AddArgumentConverter(Converters.Default);
        app.AddCollectionConverter<string, string[]>(values => values.ToArray());

        return Verify(ConfigurationAssertion.GetAssertionsAsText(app.GetConfigurationAssertions()));
    }

    [Fact]
    public Task Reports_DuplicateAliasAssertion()

    {
        var root = new RootCommand("app");
        var subCommand = new SubCommand("create");
        subCommand.SetHandler<IModel>((_, _) => Task.FromResult(0));
        root.AddSubCommand(subCommand);

        var app = new CommandLineApplication(root);
        app.ConfigureMiddleware(middleware => middleware
            .AddSwitch(
                "Version",
                "--version",
                _ => Task.FromResult<int?>(0)));
        app.ConfigureParser<IModel>(parser => parser
            .ParseOption(x => x.Option)
            .ParseRepeatableOption(x => x.Options, "--option")
            .ParseRepeatableArgument(x => x.Options2, ordinalPosition: 0, arity: Arity.One)
            .SetBinder(_ => Substitute.For<IModel>()));
        app.AddArgumentConverter(Converters.Default);
        app.AddCollectionConverter<string, string[]>(values => values.ToArray());

        return Verify(ConfigurationAssertion.GetAssertionsAsText(app.GetConfigurationAssertions()));
    }
}