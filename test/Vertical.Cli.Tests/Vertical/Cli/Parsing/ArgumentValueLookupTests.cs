using Shouldly;
using Vertical.Cli.Configuration;

namespace Vertical.Cli.Parsing;

public class ArgumentValueLookupTests
{
    [Fact]
    public void Maps_Single_Argument()
    {
        var command = new RootCommand<Model<string[]>, int>("command");
        command.AddArgument(x => x.Value);

        var unit = CreateUnit(command, ["arg"]);
        
        unit["Value"].ShouldBe(["arg"]);
    }

    [Fact]
    public void Maps_MultiValued_Arguments()
    {
        var command = new RootCommand<Model<string[]>, int>("command");
        command.AddArgument(x => x.Value, Arity.OneOrMany);

        var unit = CreateUnit(command, ["1", "2", "3"]);
        
        unit["Value"].ShouldBe(["1", "2", "3"]);
    }

    [Fact]
    public void Maps_Arguments_In_Sequence()
    {
        var command = new RootCommand<Tuple<string, string>, int>("command");
        command
            .AddArgument(x => x.Item1)
            .AddArgument(x => x.Item2);

        var unit = CreateUnit(command, ["1", "2"]);
        
        unit["Item1"].ShouldBe(["1"]);
        unit["Item2"].ShouldBe(["2"]);
    }

    [Fact]
    public void Maps_Arguments_In_Sequence_Last_Is_MultiValued()
    {
        var command = new RootCommand<Tuple<string, string>, int>("command");
        command
            .AddArgument(x => x.Item1)
            .AddArgument(x => x.Item2, Arity.OneOrMany);

        var unit = CreateUnit(command, ["1", "2", "3", "4"]);
        
        unit["Item1"].ShouldBe(["1"]);
        unit["Item2"].ShouldBe(["2", "3", "4"]);
    }

    [Fact]
    public void Maps_Switches_True()
    {
        var command = new RootCommand<Tuple<bool, bool>, int>("command");
        command
            .AddSwitch(x => x.Item1, ["-a"])
            .AddSwitch(x => x.Item2, ["-b"]);

        var unit = CreateUnit(command, ["-a", "-b"]);
        unit["Item1"].ShouldBe(["True"]);
        unit["Item2"].ShouldBe(["True"]);
    }

    [Fact]
    public void Maps_Switches_Explicit()
    {
        var command = new RootCommand<Tuple<bool, bool>, int>("command");
        command
            .AddSwitch(x => x.Item1, ["-a"])
            .AddSwitch(x => x.Item2, ["-b"]);

        var unit = CreateUnit(command, ["-a", "true", "-b"]);
        unit["Item1"].ShouldBe(["True"], Case.Insensitive);
        unit["Item2"].ShouldBe(["True"], Case.Insensitive);
    }
    
    [Fact]
    public void Maps_Switches_Explicit_Mixed()
    {
        var command = new RootCommand<Tuple<bool, bool>, int>("command");
        command
            .AddSwitch(x => x.Item1, ["-a"])
            .AddSwitch(x => x.Item2, ["-b"]);

        var unit = CreateUnit(command, ["-a", "true", "-b", "false"]);
        unit["Item1"].ShouldBe(["True"], Case.Insensitive);
        unit["Item2"].ShouldBe(["False"], Case.Insensitive);
    }

    [Fact]
    public void Maps_Options()
    {
        var command = new RootCommand<Tuple<string, string>, int>("command");
        command
            .AddOption(x => x.Item1, ["-a"])
            .AddOption(x => x.Item2, ["-b"]);

        var unit = CreateUnit(command, ["-a", "a-value", "-b:b-value"]);
        
        unit["Item1"].ShouldBe(["a-value"]);
        unit["Item2"].ShouldBe(["b-value"]);
    }
    
    [Fact]
    public void Maps_Options_MultiValued()
    {
        var command = new RootCommand<Tuple<string, string>, int>("command");
        command
            .AddOption(x => x.Item1, ["-a"])
            .AddOption(x => x.Item2, ["-b"]);

        var unit = CreateUnit(command, ["-a", "a-value-1", "-b:b-value", "-a=a-value-2"]);
        
        unit["Item1"].ShouldBe(["a-value-1", "a-value-2"]);
        unit["Item2"].ShouldBe(["b-value"]);
    }

    [Fact]
    public void Maps_Args_Before_Options()
    {
        var command = new RootCommand<Tuple<string, string, bool>, int>("command");
        command
            .AddArgument(x => x.Item1)
            .AddArgument(x => x.Item2)
            .AddSwitch(x => x.Item3, ["-s"]);

        var unit = CreateUnit(command, ["user", "password", "-s"]);
        
        unit["Item1"].ShouldBe(["user"]);
        unit["Item2"].ShouldBe(["password"]);
        unit["Item3"].ShouldBe(["True"]);
    }
    
    [Fact]
    public void Maps_Args_After_Options()
    {
        var command = new RootCommand<Tuple<string, string, bool>, int>("command");
        command
            .AddArgument(x => x.Item1)
            .AddArgument(x => x.Item2)
            .AddSwitch(x => x.Item3, ["-s"]);

        var unit = CreateUnit(command, ["-s", "user", "password"]);
        
        unit["Item1"].ShouldBe(["user"]);
        unit["Item2"].ShouldBe(["password"]);
        unit["Item3"].ShouldBe(["True"]);
    }

    [Fact]
    public void Doesnt_Map_Options_Missing_Args()
    {
        var command = new RootCommand<Model<string>, int>("command");
        command.AddOption(x => x.Value, ["-a"]);

        var unit = CreateUnit(command, ["-b"]);
        
        unit["Value"].ShouldBe([]);
    }
    
    [Fact]
    public void Enqueues_Non_Matched_Options()
    {
        var command = new RootCommand<Tuple<string, string>, int>("command");
        command
            .AddOption(x => x.Item1, ["-a"])
            .AddOption(x => x.Item2, ["-b"]);

        var unit = CreateUnit(command, ["-c", "-d"]);
        
        unit["#unmapped"].ShouldBe(["-c", "-d"]);
    }

    private static ILookup<string, string> CreateUnit<TModel, TResult>(
        RootCommand<TModel, TResult> command,
        string[] args) where TModel : class
    {
        var queue = new Queue<ArgumentSyntax>(ArgumentParser.Parse(args));
        return ArgumentValueLookup.Create(queue, command.Symbols.ToArray());
    }
}   