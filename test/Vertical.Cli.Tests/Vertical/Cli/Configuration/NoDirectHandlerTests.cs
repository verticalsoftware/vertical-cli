using Shouldly;
using Vertical.Cli.Engine;

namespace Vertical.Cli.Configuration;

public class NoDirectHandlerTests
{
    public class BaseModel
    {
        public int Value { get; set; }
    }

    public class EmptyDerivedModel : BaseModel
    {
        
    }

    [Fact]
    public void SubCommand_Validates_With_No_Direct_Options()
    {
        var root = new RootCommand<BaseModel>("root");
        root.AddOption(x => x.Value, ["--value"], scope: CliScope.Descendants);

        var sub = root.AddSubCommand<EmptyDerivedModel>("sub");
        sub.Handle(_ => 0);

        root.VerifyConfiguration();
        
        var context = CliEngine.GetContext(root, ["sub"]);
        context.TryGetCallSite<EmptyDerivedModel>(out var callSite).ShouldBeTrue();
        callSite.ShouldNotBeNull();
    }
    
    
}