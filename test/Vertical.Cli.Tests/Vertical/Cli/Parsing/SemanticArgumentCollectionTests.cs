using NSubstitute;
using Vertical.Cli.Binding;
using Vertical.Cli.Configuration;
using Vertical.Cli.Test;

namespace Vertical.Cli.Parsing;

public class SemanticArgumentCollectionTests
{
    private static readonly ICommandDefinition Command = Substitute.For<ICommandDefinition>();
    private static readonly Dictionary<string, SymbolDefinition> Symbols = new()
    {
        ["--switch"] = Factories.CreateSymbol<bool>(SymbolKind.Switch, "--switch", "-s"),
        ["--shape"] = Factories.CreateSymbol<string>(SymbolKind.Option, "--shape")
    };
    private readonly SemanticArgumentCollection _instance = Setup();
    private readonly SemanticArgument[] _arguments;

    public SemanticArgumentCollectionTests() => _arguments = _instance.ToArray();
    
    [Fact]
    public void OperandNullWhenKnownSymbol()
    {
        Assert.Null(_arguments[0].OperandSourceSyntax);
    }

    [Fact]
    public void SimpleSyntaxHasNoSymbol()
    {
        Assert.Null(_arguments[2].OptionSymbol);
    }

    [Fact]
    public void OptionSyntaxMatchesSymbolWithId()
    {
        Assert.Equal("--switch", _arguments[1].OptionSymbol!.Id);
    }

    [Fact]
    public void OptionSyntaxMatchesSymbolWithAlias()
    {
        Assert.Equal("--switch", _arguments[3].OptionSymbol!.Id);
    }

    [Fact]
    public void PosixGroupExpanded()
    {
        Assert.Equal("-a", _arguments[4].ArgumentSyntax.Text);
        Assert.Equal("-b", _arguments[5].ArgumentSyntax.Text);
        Assert.Equal("-c", _arguments[6].ArgumentSyntax.Text);
    }

    [Fact]
    public void AssignedOptionWithOperand()
    {
        Assert.Equal("--shape", _arguments[7].OptionSymbol!.Id);
        Assert.Equal("square", _arguments[7].OperandValue);
    }

    [Fact]
    public void AssignedOptionWithOperandFromTrailingArg()
    {
        Assert.Equal("--shape", _arguments[8].OptionSymbol!.Id);
        Assert.Equal("circle", _arguments[8].OperandValue);
    }

    [Fact]
    public void CreatesNonAttachedInstances()
    {
        Assert.Null(_arguments[2].OptionSymbol);
        Assert.Null(_arguments[9].OptionSymbol);
    }

    [Fact]
    public void FindsTermination()
    {
        Assert.True(_arguments[11].Terminated);
        Assert.True(_arguments[12].Terminated);
    }

    [Fact]
    public void FindsNonIdentifier()
    {
        Assert.Equal(SymbolSyntaxType.NonIdentifier, _arguments[10].ArgumentSyntax.Type);
        Assert.Null(_arguments[10].OptionSymbol);
    }

    [Fact]
    public void IgnoresSymbolMatchingForTerminatedArg()
    {
        Assert.Null(_arguments[11].OptionSymbol);
    }

    [Fact]
    public void GetsOptionArguments()
    {
        var matched = _instance.GetOptionArguments(Symbols["--shape"]).ToArray();
        Assert.Equal(2, matched.Length);
        Assert.Equal("square", matched[0].OperandValue);
        Assert.Equal("circle", matched[1].OperandValue);
    }

    [Fact]
    public void GetsValueArguments()
    {
        // Accept so collection is in correct state
        foreach (var arg in Symbols.Values.SelectMany(symbol => _instance.GetOptionArguments(symbol)))
        {
            arg.Accept(); 
            if (arg.OptionSymbol is { Kind: SymbolKind.Option })
            {
                arg.AcceptOperand();
            }
        }

        var multiValueSymbol = new SymbolDefinition<string>(
            SymbolKind.Argument,
            Command,
            () => ArgumentBinder<string>.Instance,
            0, "arg", Array.Empty<string>(), Arity.OneOrMany,
            null, SymbolScope.Parent, null, null);

        var arguments = _instance.GetValueArguments(multiValueSymbol);
        
        Assert.Collection(arguments,
            arg => Assert.Equal("--option", arg.ArgumentSyntax.Text),
            arg => Assert.Equal("red", arg.ArgumentSyntax.Text),
            arg => Assert.Equal("-a", arg.ArgumentSyntax.Text),
            arg => Assert.Equal("-b", arg.ArgumentSyntax.Text),
            arg => Assert.Equal("-c", arg.ArgumentSyntax.Text),
            arg => Assert.Equal("--invalid=", arg.ArgumentSyntax.Text),
            arg => Assert.Equal("triangle", arg.ArgumentSyntax.Text),
            arg => Assert.Equal("--shape=octagon", arg.ArgumentSyntax.Text)
            );
    }

    [Fact]
    public void ArgumentRemovedWhenAccepted()
    {
        var switchOption = Symbols["--switch"];
        var args = _instance.GetOptionArguments(switchOption).ToArray();
        foreach (var arg in args)
        {
            arg.Accept();
        }

        Assert.Empty(_instance.Unaccepted.Where(arg => ReferenceEquals(arg.OptionSymbol, switchOption)));
    }

    [Fact]
    public void OperandRemovedWhenAccepted()
    {
        var shapeOption = Symbols["--shape"];
        foreach (var arg in _instance.GetOptionArguments(shapeOption))
        {
            arg.AcceptOperand();
        }
        
        Assert.Empty(_instance.Unaccepted.Where(arg => arg.ArgumentSyntax.Text == "circle"));
    }
    
    private static SemanticArgumentCollection Setup()
    {
        var arguments = new[]
        {
            "--option", 
            "--switch", 
            "red",  
            "-s",   
            "-abc", 
            "--shape=square", // @7
            "--shape",
            "circle",
            "--invalid=",
            "--", // not an argument
            "triangle", // 10
            "--shape=octagon"
        };

        var syntax = arguments.Select(SymbolSyntax.Parse).ToArray();

        return new SemanticArgumentCollection(Symbols.Values, syntax);
    }
}