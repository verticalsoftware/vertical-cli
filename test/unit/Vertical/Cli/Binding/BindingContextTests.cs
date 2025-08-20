using NSubstitute;
using Shouldly;
using Vertical.Cli.Configuration;
using Vertical.Cli.Conversion;
using Vertical.Cli.Invocation;
using Vertical.Cli.IO;
using Vertical.Cli.Parsing;

namespace Vertical.Cli.Binding;

public class BindingContextTests
{
    public record Options(bool LogVerbose, int Timeout, string[] Tags, Coordinate Location);

    public record Coordinate(double Lat, double Long);

    [Fact]
    public void GetValue_Returns_Switch_Default()
    {
        var unit = Setup((model, _) => model.AddSwitch(x => x.LogVerbose), []);
        unit.GetValue(x => x.LogVerbose, Converters.Boolean).ShouldBeFalse();
    }

    [Fact]
    public void GetValue_Returns_Explicit_Value()
    {
        var unit = Setup((model, _) => model.AddOption(x => x.Timeout, setBindingOptions: value => value.SetValue(30)), []);
        unit.GetValue(x => x.Timeout, Converters.Integer).ShouldBe(30);
    }

    [Fact]
    public void GetValue_Returns_Parsed_Value()
    {
        var unit = Setup((model, _) => model.AddOption(x => x.Timeout), ["--timeout=15"]);
        unit.GetValue(x => x.Timeout, Converters.Integer).ShouldBe(15);
    }
    
    [Fact]
    public void GetValue_Returns_Parsed_Value_Using_Converter()
    {
        var unit = Setup((model, config) =>
        {
            model.AddOption(x => x.Location);
            config.AddValueConverter(str =>
            {
                var split = str.Split(',');
                return new Coordinate(double.Parse(split[0]), double.Parse(split[1]));
            });
        }, ["--location=39.742043,-104.991531"]);
        
        unit.GetValue(x => x.Location, null).ShouldBe(new Coordinate(39.742043, -104.991531));
    }

    [Fact]
    public void GetCollection_Returns_Default_Value()
    {
        var unit = Setup((model, _) => model.AddCollectionArgument(
            x => x.Tags,
            0,
            setBindingOptions: value => value.SetDefaultValue(["tag"])), []);
        unit
            .GetCollectionValue<string, string[]>(x => x.Tags, Converters.Default, values => [..values])
            .ShouldBe(["tag"]);
    }
    
    [Fact]
    public void GetCollection_Returns_Explicit_Value()
    {
        var unit = Setup((model, _) => model.AddCollectionArgument(
            x => x.Tags,
            0,
            setBindingOptions: value => value.SetValue(["explicit-tag"])), ["tag"]);
        unit
            .GetCollectionValue<string, string[]>(x => x.Tags, Converters.Default, values => [..values])
            .ShouldBe(["explicit-tag"]);
    }
    
    [Fact]
    public void GetCollection_Returns_Parsed_Values()
    {
        var unit = Setup((model, _) => model.AddCollectionArgument(
            x => x.Tags,
            0), ["tag-1", "tag-2"]);
        unit
            .GetCollectionValue<string, string[]>(x => x.Tags, Converters.Default, values => [..values])
            .ShouldBe(["tag-1", "tag-2"]);
    }

    private BindingContext<Options> Setup(
        Action<ModelConfiguration<Options>, CommandLineBuilder> configure,
        string[] arguments)
    {
        var handler = new CommandHandler<Options>((_, _) => Task.FromResult(0));
        var rootCommand = new RootCommand<Options>("root", handler);
        var parser = new Parser(new CommandLineOptions());
        var builder = new CommandLineBuilder(rootCommand);
        var modelConfiguration = new ModelConfiguration<Options>(parser);
        var errors = new List<UsageError>();
        
        configure(modelConfiguration, builder);

        var contextBuilder = new HandlerContextBuilder<Options>(rootCommand, handler);
        modelConfiguration.ConfigureContext(contextBuilder);

        var parserResult = ParseResult.Create(
            parser,
            new LinkedList<Token>(parser.ParseArguments(arguments)),
            modelConfiguration.Symbols.ToArray(),
            errors);

        var context = new BindingContext<Options>(
            builder,
            contextBuilder.Bindings,
            parserResult,
            errors,
            null, 
            Substitute.For<TextReader>());

        return context;
    }
}