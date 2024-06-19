using Shouldly;

namespace Vertical.Cli.Configuration;

public class CliExtensionsTests
{
    [Fact]
    public void AggregateSymbols_Returns_Expected()
    {
        var root = new RootCommand<Model<bool>>("parent");
        root.AddSwitch(x => x.Value, ["-a"], scope: CliScope.Self)
            .AddSwitch(x => x.Value, ["-b"], scope: CliScope.Descendants)
            .AddSwitch(x => x.Value, ["-c"], scope: CliScope.SelfAndDescendants);

        var child = root.AddSubCommand<Model<bool>>("child");
        child.AddSwitch(x => x.Value, ["-d"], scope: CliScope.Self)
             .AddSwitch(x => x.Value, ["-e"], scope: CliScope.Descendants)
             .AddSwitch(x => x.Value, ["-f"], scope: CliScope.SelfAndDescendants);
        
        var grandchild = child.AddSubCommand<Model<bool>>("grandchild");
        grandchild.AddSwitch(x => x.Value, ["-g"], scope: CliScope.Self)
                  .AddSwitch(x => x.Value, ["-h"], scope: CliScope.Descendants)
                  .AddSwitch(x => x.Value, ["-i"], scope: CliScope.SelfAndDescendants);

        
        root.AggregateSymbols().Select(symbol => symbol.Names[0]).Order().ShouldBe(["-a", "-c"]);
        child.AggregateSymbols().Select(symbol => symbol.Names[0]).Order().ShouldBe(["-b", "-c", "-d", "-f"]);
        grandchild.AggregateSymbols().Select(symbol => symbol.Names[0]).Order().ShouldBe(["-b", "-c", "-e", "-f", "-g", "-i"]);
    }
}