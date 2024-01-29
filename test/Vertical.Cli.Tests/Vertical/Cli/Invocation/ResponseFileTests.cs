using Vertical.Cli.Binding;

namespace Vertical.Cli.Invocation;

public class ResponseFileTests
{
    public record Model(
        string Path,
        bool NoRestore,
        bool NoSymbols,
        string? ApiKey,
        string? Source,
        [BindTo("--config")] string Configuration);
    
    [Fact]
    public void InvocationReadsAndMergesResponseFile()
    {
        var root = RootCommand.Create<Model, int>(
            "test",
            cmd =>
            {
                cmd
                    .AddArgument<string>("path", Arity.One)
                    .AddSwitch("--no-restore")
                    .AddSwitch("--no-symbols")
                    .AddOption<string?>("--api-key")
                    .AddOption<string?>("--source")
                    .AddOption<string>("--config", ["-c"], defaultProvider: () => "Debug");

                cmd.SetResponseFileOption();

                cmd.SetHandler(model => 
                {
                    Assert.Equal("/var/lib/MyProject.csproj", model.Path);
                    Assert.True(model.NoRestore);
                    Assert.True(model.NoSymbols);
                    Assert.Equal("*the_secret*", model.ApiKey);
                    Assert.Null(model.Source);
                    Assert.Equal("Debug", model.Configuration);
                    
                    return 1;
                });
            });

        string[] args = ["/var/lib/MyProject.csproj", "--silent:Resources/test-arguments.rsp"];

        var callSiteContext = CallSiteContext.Create(root, args, 0);
        var callSite = callSiteContext.BindModelToCallSite(bindingContext => new Model(
            bindingContext.GetValue<string>("Path"),
            bindingContext.GetValue<bool>("NoRestore"),
            bindingContext.GetValue<bool>("NoSymbols"),
            bindingContext.GetValue<string?>("ApiKey"),
            bindingContext.GetValue<string?>("Source"),
            bindingContext.GetValue<string>("--config")));
        
        Assert.Equal(1, callSite(CancellationToken.None));
    }
}