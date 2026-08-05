using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Vertical.Cli.Configuration;
using Vertical.Cli.Invocation;

namespace Vertical.Cli.UnitTests.Invocation;

public class HandlerServiceContextTests
{
    private interface IModel
    {
    }

    private record Model : IModel;

    private sealed class Handler : IHandler<IModel>
    {
        public bool Called { get; private set; }
        
        /// <inheritdoc />
        public Task<int> HandleAsync(IModel options, CancellationToken cancellationToken)
        {
            Called = true;
            return Task.FromResult(0);
        }
    }
    
    [Fact]
    public async Task Create_Wrapped_Returns_ServiceContext()
    {
        var command = new RootCommand("test");
        var called = false;
        command.SetHandler((IModel _, CancellationToken _) =>
        {
            called = true;
            return Task.FromResult(0);
        });

        var app = new CommandLineApplication(command);
        app.ConfigureParser<IModel>(builder => builder.SetBinder(_ => new Model()));
        _ = await app.RunAsync([]);
        called.ShouldBeTrue();
    }

    [Fact]
    public async Task Create_Factory_Context_Returns_ServiceContext()
    {
        var command = new RootCommand("test");
        var handler = new Handler();
        command.SetHandler(_ => handler);

        var app = new CommandLineApplication(command);
        app.ConfigureParser<IModel>(builder => builder.SetBinder(_ => new Model()));
        _ = await app.RunAsync([]);
        handler.Called.ShouldBeTrue();
    }

    [Fact]
    public async Task Create_Service_Resolved_Context_Returns_ServiceContext()
    {
        var command = new RootCommand("test");
        var handler = new Handler();
        command.SetHandler<IModel, Handler>();

        var app = new CommandLineApplication(command);
        app.ConfigureParser<IModel>(builder => builder.SetBinder(_ => new Model()));

        var services = new ServiceCollection();
        services.AddSingleton(handler);
        app.UseServices(_ => services.BuildServiceProvider());
        _ = await app.RunAsync([]);
        handler.Called.ShouldBeTrue();
    }
}