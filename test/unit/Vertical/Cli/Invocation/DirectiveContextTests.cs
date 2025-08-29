using NSubstitute;
using Shouldly;
using Vertical.Cli.Configuration;
using Vertical.Cli.IO;

namespace Vertical.Cli.Invocation;

public class DirectiveContextTests
{
    [Fact]
    public async Task AddDirective_Invokes_Handler_With_Token_Removal()
    {
        var context = await CreateAndInvokeContext(
            middleware => middleware.AddDirectiveHandler(args =>
            {
                if (args.Token.Text == "[test]")
                {
                    args.DequeueToken();
                }

                return Task.CompletedTask;
            }),
            ["[test]", "--option", "argument"]);
        
        context.TokenList.First?.Value.Text.ShouldBe("--option");
    }
    
    [Fact]
    public async Task AddDirective_Invokes_Handler_With_No_Token_Removal()
    {

        var context = await CreateAndInvokeContext(
            middleware => middleware.AddDirectiveHandler(args =>
            {
                if (args.Token.Text == "[test]")
                    args.DequeueToken();
                
                return Task.CompletedTask;
            }),
            ["[not-test]", "--option", "argument"]);
        
        context.TokenList.First?.Value.Text.ShouldBe("[not-test]");
    }

    private async Task<InvocationContext> CreateAndInvokeContext(
        Action<MiddlewareConfiguration> configure,  
        string[] arguments)
    {
        var builder = new CommandLineBuilder(new RootCommand("test"));
        builder.ConfigureMiddleware(middleware => middleware.Clear());
        builder.ConfigureMiddleware(configure);
        
        IRootConfiguration configuration = builder;

        var pipeline = configuration.CreateMiddlewarePipeline([]);
        var context = InvocationContext.Create(builder, arguments, Substitute.For<IConsole>());

        await pipeline(context);

        return context;
    }
}