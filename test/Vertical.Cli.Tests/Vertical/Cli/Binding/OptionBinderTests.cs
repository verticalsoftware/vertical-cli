using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Vertical.Cli.Configuration;
using Vertical.Cli.Test;

namespace Vertical.Cli.Binding;

public class OptionBinderTests
{
    [Fact]
    public void UsesDefaultProvider()
    {
        var provider = () => "value";
        var option = CreateOption(Arity.ZeroOrOne, provider);
        var context = SetupBindingContext(CreateOption(Arity.ZeroOrOne, provider));
        var binding = (ArgumentBinding<string>)OptionBinder<string>.Instance.CreateBinding(context, option);

        Assert.Equal(provider(), binding.Values.Single());
    }

    [Fact]
    public void ReturnsNoValues()
    {
        var option = CreateOption(Arity.ZeroOrOne);
        var context = SetupBindingContext(option);
        var binding = (ArgumentBinding<string>)OptionBinder<string>.Instance.CreateBinding(context, option);

        Assert.Empty(binding.Values);
    }

    [Fact]
    public void ThrowsArityNotMet()
    {
        var option = CreateOption(Arity.One);
        var context = SetupBindingContext(option);

        Assert.Throws<CliArityException>(() => OptionBinder<string>.Instance.CreateBinding(context, option));
    }
    
    [Fact]
    public void ThrowsArityExceeded()
    {
        var option = CreateOption(Arity.One);
        var context = SetupBindingContext(option, "--option=red", "--option=green");

        Assert.Throws<CliArityException>(() => OptionBinder<string>.Instance.CreateBinding(context, option));
    }

    [Fact]
    public void AcceptsOption()
    {
        var option = CreateOption(Arity.One);
        var context = SetupBindingContext(option, "--option=red");
        _ = OptionBinder<string>.Instance.CreateBinding(context, option);
        
        Assert.Empty(context.SemanticArguments.Unaccepted);
    }

    [Fact]
    public void AcceptsOperand()
    {
        var option = CreateOption(Arity.One);
        var context = SetupBindingContext(option, "--option", "red");
        _ = OptionBinder<string>.Instance.CreateBinding(context, option);
        
        Assert.Empty(context.SemanticArguments.Unaccepted);
    }

    [Fact]
    public void AssignsOptionValue()
    {
        var option = CreateOption(Arity.One);
        var context = SetupBindingContext(option, "--option=red");
        var binding = (ArgumentBinding<string>)OptionBinder<string>.Instance.CreateBinding(context, option);
        
        Assert.Equal("red", binding.Values.Single());
    }
    
    [Fact]
    public void AssignsOperandValue()
    {
        var option = CreateOption(Arity.One);
        var context = SetupBindingContext(option, "--option", "red");
        var binding = (ArgumentBinding<string>)OptionBinder<string>.Instance.CreateBinding(context, option);
        
        Assert.Equal("red", binding.Values.Single());
    }

    [Fact]
    public void AssignMultipleValues()
    {
        var option = CreateOption(Arity.ZeroOrMany);
        var context = SetupBindingContext(option, "--option=red", "--option=green", "--option", "blue");
        var binding = (ArgumentBinding<string>)OptionBinder<string>.Instance.CreateBinding(context, option);
        
        Assert.Collection(binding.Values,
            str => Assert.Equal("red", str),
            str => Assert.Equal("green", str),
            str => Assert.Equal("blue", str));
    }

    [Fact]
    public void ThrowsOptionMissingOperand()
    {
        var option = CreateOption(Arity.One);
        var context = SetupBindingContext(option, "--option");

        Assert.Throws<CliMissingOperandException>(() => OptionBinder<string>.Instance.CreateBinding(context, option));
    }

    [Fact]
    public void AcceptsMultipleValuesAndOperands()
    {        
        var option = CreateOption(Arity.ZeroOrMany);
        var context = SetupBindingContext(option, "--option=red", "--option=green", "--option", "blue");
        _ = (ArgumentBinding<string>)OptionBinder<string>.Instance.CreateBinding(context, option);
        
        Assert.Empty(context.SemanticArguments.Unaccepted);
    }
    
    private static SymbolDefinition CreateOption(Arity arity, Func<string>? defaultProvider = null)
    {
        return Factories.CreateSymbol(
            Factories.DefaultCommand,
            SymbolType.Option,
            id: "--option",
            arity: arity,
            defaultProvider: defaultProvider);
    }
    
    private static IBindingContext SetupBindingContext(SymbolDefinition option, params string[] args)
    {
        var command = Substitute.For<ICommandDefinition>();
        command.Symbols.Returns(new[] { option });
        command.Parent.ReturnsNull();
        
        return new RuntimeBindingContext(new CliOptions(), 
            command, 
            args, 
            args);
    }
}