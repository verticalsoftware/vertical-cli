using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Vertical.Cli.Configuration;
using Vertical.Cli.Invocation;

namespace Vertical.Cli.DependencyInjection.UnitTests;

public class BehaviorTests
{
    public record Options {}

    public class Handler : IHandler<Options>
    {
        /// <inheritdoc />
        public Task<int> HandleAsync(Options options, CancellationToken cancellationToken)
        {
            return Task.FromResult(-1);
        }
    }

    [Fact]
    public async Task Run_Invokes_Instance_From_IServiceProvider_Resolution()
    {
        var command = new RootCommand("test");
        command.SetHandler(provider => provider.GetRequiredService<Handler>());
        
        var app = new CommandLineApplication(command);
        app.Services.AddSingleton<Handler>();
        app.ConfigureParser<Options>(builder => builder.SetBinder(_ => new Options()));

        (await app.RunAsync([])).ShouldBe(-1);
    }

    [Fact]
    public async Task Run_Invokes_Instance_From_Concrete_Type_Resolution()
    {
        var command = new RootCommand("test");
        command.SetHandler<Options, Handler>();

        var app = new CommandLineApplication(command);
        app.Services.AddSingleton<Handler>();
        app.ConfigureParser<Options>(builder => builder.SetBinder(_ => new Options()));

        (await app.RunAsync([])).ShouldBe(-1);
    }
}