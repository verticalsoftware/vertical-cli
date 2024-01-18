using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Vertical.Cli.Configuration;
using Vertical.Cli.Test;

namespace Vertical.Cli.Binding;

public class SwitchBinderTests
{
    private static readonly SymbolDefinition SwitchSymbol = Factories.CreateSymbol<bool>(
        Factories.DefaultCommand,
        SymbolType.Switch,
        id: "--switch",
        arity: Arity.ZeroOrOne);
    
    [Fact]
    public void InfersTrueBindingValue()
    {
        var context = SetupBindingContext("--switch");
        var binding = (ArgumentBinding<bool>)SwitchBinder.Instance.CreateBinding(context, SwitchSymbol);
        
        Assert.True(binding.Values.Single());
    }

    [Theory]
    [InlineData(true), InlineData(false)]
    public void ReturnsExpectedWithAttachedOperandValue(bool value)
    {
        var context = SetupBindingContext($"--switch={value}");
        var binding = (ArgumentBinding<bool>)SwitchBinder.Instance.CreateBinding(context, SwitchSymbol);
        
        Assert.Equal(value, binding.Values.Single());
    }
    
    [Theory]
    [InlineData(true), InlineData(false)]
    public void ReturnsExpectedTrailingOperandValue(bool value)
    {
        var context = SetupBindingContext("--switch", value.ToString().ToLower());
        var binding = (ArgumentBinding<bool>)SwitchBinder.Instance.CreateBinding(context, SwitchSymbol);
        
        Assert.Equal(value, binding.Values.Single());
    }

    [Fact]
    public void ThrowsWhenAttachedOperandValueIsNotBool()
    {
        var context = SetupBindingContext("--switch=not-boolean");

        Assert.ThrowsAny<CliArgumentException>(() => SwitchBinder.Instance.CreateBinding(context, SwitchSymbol));
    }

    [Fact]
    public void DoesNotAcceptNonBooleanOperandValue()
    {
        var context = SetupBindingContext("--switch", "not-boolean");
        _ = SwitchBinder.Instance.CreateBinding(context, SwitchSymbol);
        var unused = context.SemanticArguments.Unaccepted;
        
        Assert.Equal("not-boolean", unused.Single().ArgumentSyntax.Text);
    }

    [Fact]
    public void AcceptsArgument()
    {
        var context = SetupBindingContext("--switch");
        _ = SwitchBinder.Instance.CreateBinding(context, SwitchSymbol);
        
        Assert.Empty(context.SemanticArguments.Unaccepted);
    }

    [Fact]
    public void AcceptsOperand()
    {
        var context = SetupBindingContext("--switch", "false");
        _ = SwitchBinder.Instance.CreateBinding(context, SwitchSymbol);
        
        Assert.Empty(context.SemanticArguments.Unaccepted);
    }

    [Fact]
    public void UsesDefaultProvider()
    {
        var provider = new Provider<bool>(true);
        var symbol = Factories.CreateSymbol<bool>(
            Factories.DefaultCommand,
            SymbolType.Switch,
            "--switch",
            defaultProvider: provider);
        _ = SwitchBinder.Instance.CreateBinding(SetupBindingContext("--switch"), symbol);
        
        Assert.True(provider.Called);
    }

    [Fact]
    public void InfersFalseBindingValue()
    {
        var context = SetupBindingContext();
        
        var binding = (ArgumentBinding<bool>)SwitchBinder.Instance.CreateBinding(context, SwitchSymbol);
        
        Assert.False(binding.Values.Single());
    }
    
    private static IBindingContext SetupBindingContext(params string[] args)
    {
        var command = Substitute.For<ICommandDefinition>();
        command.Symbols.Returns(new[] { SwitchSymbol });
        command.Parent.ReturnsNull();
        
        return new RuntimeBindingContext(new CliOptions(), 
            command, 
            args, 
            args);
    }
}