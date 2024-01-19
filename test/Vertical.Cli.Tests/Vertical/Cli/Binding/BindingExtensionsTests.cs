using NSubstitute;
using Vertical.Cli.Configuration;
using Vertical.Cli.Test;
using Vertical.Cli.Validation;

namespace Vertical.Cli.Binding;

public class BindingExtensionsTests
{
    [Fact]
    public void ConvertsWithDefaultInstance()
    {
        var symbol = (SymbolDefinition<int>)Factories.CreateSymbol<int>(
            Factories.DefaultCommand,
            SymbolKind.Argument,
            "arg");
        var args = new[] { "100" };
        var context = new RuntimeBindingContext(new CliOptions(),
            Factories.DefaultCommand,
            args, args);
        var value = context.GetBindingValue(symbol, args[0]);
        
        Assert.Equal(100, value);
    }

    [Fact]
    public void ConvertsWithOptionsConverter()
    {
        var symbol = (SymbolDefinition<int>)Factories.CreateSymbol<int>(
            Factories.DefaultCommand,
            SymbolKind.Argument,
            "arg");
        var args = new[] { "100" };
        var cliOptions = new CliOptions();
        cliOptions.AddConverter(_ => 101);
        var context = new RuntimeBindingContext(cliOptions,
            Factories.DefaultCommand,
            args, args);
        var value = context.GetBindingValue(symbol, args[0]);
        
        Assert.Equal(101, value);
    }

    [Fact]
    public void ThrowsValueConversionFailed()
    {
        var symbol = (SymbolDefinition<int>)Factories.CreateSymbol<int>(
            Factories.DefaultCommand,
            SymbolKind.Argument,
            "arg");
        var args = new[] { "string" };
        var context = new RuntimeBindingContext(new CliOptions(),
            Factories.DefaultCommand,
            args, args);

        Assert.Throws<CliValueConversionException>(() => context.GetBindingValue(
            symbol, args[0]));
    }

    [Fact]
    public void ReturnsValueOnValidationSuccess()
    {
        var validator = Validator.Configure<string>(x => x.MinimumLength(1));
        var symbol = (SymbolDefinition<string>)Factories.CreateSymbol(
            Factories.DefaultCommand,
            SymbolKind.Argument,
            "arg",
            validator: validator);
        var args = new[] { "arg" };
        var context = new RuntimeBindingContext(new CliOptions(),
            Factories.DefaultCommand,
            args, args);

        Assert.Equal("arg", context.GetBindingValue(symbol, "arg"));
    }

    [Fact]
    public void InvokesValidator()
    {
        var validator = Validator.Configure<string>(x => x.MinimumLength(5));
        var symbol = (SymbolDefinition<string>)Factories.CreateSymbol(
            Factories.DefaultCommand,
            SymbolKind.Argument,
            "arg",
            validator: validator);
        var args = new[] { "str" };
        var context = new RuntimeBindingContext(new CliOptions(),
            Factories.DefaultCommand,
            args, args);

        Assert.Throws<CliValidationFailedException>(() => context.GetBindingValue(
            symbol, args[0]));
    }
}