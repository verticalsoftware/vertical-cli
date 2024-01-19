using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Vertical.Cli.Configuration;
using Vertical.Cli.Test;

namespace Vertical.Cli.Binding;

public class ArgumentBinderTests
{
    [Fact]
    public void SelectsSingle()
    {
        var argument = CreateSymbol(Arity.One);
        var context = SetupBindingContext(argument, "red", "green", "blue");
        var binding = (ArgumentBinding<string>)ArgumentBinder<string>.Instance.CreateBinding(context, argument);

        Assert.Equal("red", binding.Values.Single());
    }
    
    [Fact]
    public void SelectsUpToMaxCount()
    {
        var argument = CreateSymbol(new Arity(0, 2));
        var context = SetupBindingContext(argument, "red", "green", "blue");
        var binding = (ArgumentBinding<string>)ArgumentBinder<string>.Instance.CreateBinding(context, argument);

        Assert.Collection(binding.Values,
            arg => Assert.Equal("red", arg),
            arg => Assert.Equal("green", arg));
    }

    [Fact]
    public void AcceptsArguments()
    {
        var argument = CreateSymbol(Arity.One);
        var context = SetupBindingContext(argument, "red", "green", "blue");
        _ = (ArgumentBinding<string>)ArgumentBinder<string>.Instance.CreateBinding(context, argument);
        
        Assert.Collection(context.SemanticArguments.Unaccepted,
            arg => Assert.Equal("green", arg.ArgumentSyntax.Text),
            arg => Assert.Equal("blue", arg.ArgumentSyntax.Text));
    }
    
    [Fact]
    public void UsesDefaultProvider()
    {
        var argument = CreateSymbol(Arity.One, defaultProvider: () => "black");
        var context = SetupBindingContext(argument);
        var binding = (ArgumentBinding<string>)ArgumentBinder<string>.Instance.CreateBinding(context, argument);

        Assert.Equal("black", binding.Values.Single());
    }
    
    private static SymbolDefinition CreateSymbol(Arity arity, Func<string>? defaultProvider = null)
    {
        return Factories.CreateSymbol(
            Factories.DefaultCommand,
            SymbolKind.Argument,
            id: "arg",
            arity: arity,
            defaultProvider: defaultProvider);
    }
    
    private static IBindingContext SetupBindingContext(SymbolDefinition symbol, params string[] args)
    {
        var command = Substitute.For<ICommandDefinition>();
        command.Symbols.Returns(new[] { symbol });
        command.Parent.ReturnsNull();
        
        return new RuntimeBindingContext(new CliOptions(), 
            command, 
            args, 
            args);
    }
}