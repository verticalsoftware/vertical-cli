using Shouldly;
using Vertical.Cli.Binding;
using Vertical.Cli.Configuration;
using Vertical.Cli.Help;
using Vertical.Cli.Invocation;

namespace Vertical.Cli.Parsing;

public class ParseResultTests
{
    private static readonly ISymbolBinding[] Bindings =
    [
        new MyBinding(SymbolBehavior.Switch, "LogVerbose", Arity.One, ["-v", "--log-verbose"], HasBindingOptions: true),
        new MyBinding(SymbolBehavior.Option, "Host", Arity.One, ["--host"]),
        new MyBinding(SymbolBehavior.Option, "ApiKey", Arity.One, ["-k", "--api-key"]),
        new MyBinding(SymbolBehavior.Argument, "Path", Arity.One, ["PATH"], Precedence: 0),
        new MyBinding(SymbolBehavior.Argument, "Patterns", Arity.ZeroOrMore, ["PATTERN"], Precedence: 1),
    ];

    private record MyBinding(SymbolBehavior Behavior,
        string BindingName,
        Arity Arity,
        string[] Aliases,
        bool HasBindingOptions = false,
        int Precedence = 0) 
        : ISymbolBinding
    {
        /// <inheritdoc />
        public Type ModelType => typeof(object);

        /// <inheritdoc />
        public Type ValueType => typeof(int);

        /// <inheritdoc />
        public SymbolHelpTag? HelpTag => null;
    }

    [Fact]
    public void ParseResult_Gets_Inferred_Switch_Value() => Run(["-v"], [("LogVerbose", [bool.TrueString])]);

    [Fact]
    public void ParseResult_Gets_Attached_Switch_Value()
    {
        Run(["-v:true"], [("LogVerbose", ["true"])]);
        Run(["-v:false"], [("LogVerbose", ["false"])]);
    }

    [Fact]
    public void ParseResult_Gets_Attached_Option_Value()
    {
        Run(["--host:vertical.com"], [("Host", ["vertical.com"])]);
    }
    
    [Fact]
    public void ParseResult_Gets_Trailing_Option_Value()
    {
        Run(["--host", "vertical.com"], [("Host", ["vertical.com"])]);
    }

    [Fact]
    public void ParseResult_Gets_Terminated_Option_Value()
    {
        Run(["--host", "--", "vertical.com"], [("Host", ["vertical.com"])]);
    }
    
    [Fact]
    public void ParseResult_Gets_Terminated_Option_Value_After_Other_Options()
    {
        Run(["--host", "--api-key=secret!", "--", "vertical.com"], [("Host", ["vertical.com"])]);
    }

    [Fact]
    public void ParseResult_Gets_Argument_Values()
    {
        Run(["/var/temp", "[a-z]+", "[A-Z]?"], [("Path", ["/var/temp"]), ("Patterns", ["[a-z]+", "[A-Z]?"])]);
    }

    [Fact]
    public void ParseResult_Sets_No_Arity_Error_For_Switch()
    {
        var result = GetParseResult([], [Bindings[0]]);
        result.Errors.ShouldBeEmpty();
    }

    [Fact]
    public void ParseResult_Sets_Minimum_ArityError()
    {
        var result = GetParseResult([], [Bindings[1]]);
        result.Errors.Single().ShouldBeOfType<ArityError>();
    }
    
    [Fact]
    public void ParseResult_Sets_Maximum_ArityError()
    {
        var result = GetParseResult(["--host:site1.com", "--host:site2.com"], [Bindings[1]]);
        result.Errors.Single().ShouldBeOfType<ArityError>();
    }
    
    [Fact]
    public void ParseResult_Sets_MissingParameterError()
    {
        var result = GetParseResult(["--host"], [Bindings[1]]);
        result.Errors.ShouldContain(error => error is MissingParameterError);
    }
    
    [Fact]
    public void ParseResult_Sets_InvalidSwitchParameterError()
    {
        var result = GetParseResult(["-v:not-a-boolean"], [Bindings[0]]);
        result.Errors.ShouldContain(error => error is InvalidSwitchParameterError);
    }

    private static void Run(string[] args, (string BindingName, string[] Values)[] expected)
    {
        var result = GetParseResult(args);

        foreach (var pair in expected)
        {
            result.TryGetValues(pair.BindingName, out var values).ShouldBeTrue();
            values.ShouldBe(pair.Values);
        }
    }

    private static ParseResult GetParseResult(string[] args, ISymbolBinding[]? bindings = null)
    {
        var parser = new Parser(new CommandLineOptions { ParseWindowsStyleOptions = false });
        var tokens = parser.ParseArguments(args);
        return ParseResult.Create(parser, new LinkedList<Token>(tokens), bindings ?? Bindings, []);
    }
}