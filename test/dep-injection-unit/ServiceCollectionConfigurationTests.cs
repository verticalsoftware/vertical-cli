using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Vertical.Cli.Configuration;
using Vertical.Cli.Invocation;

namespace Vertical.Cli.DependencyInjection.UnitTests;

public class ServiceCollectionConfigurationTests
{
    private const string TestString = "test-string";
    private const int TestInt = 100;

    private record Model;

    private class Handler(
        List<string> strings,
        List<int> integers)
        : IHandler<Model>
    {
        /// <inheritdoc />
        public Task<int> HandleAsync(Model options, CancellationToken cancellationToken)
        {
            strings.Add(TestString);
            integers.Add(TestInt);
            return Task.FromResult(0);
        }
    }
    
    [Fact]
    public async Task ConfigureServices_Runs_All_Chained_Delegates()
    {
        var command = new RootCommand("test");
        command.SetHandlerService<Model, Handler>();

        var app = new CommandLineApplication(command);
        var strings = new List<string>();
        var integers = new List<int>();

        app.ConfigureServices(services => services.AddSingleton<Handler>());
        app.ConfigureServices(services => services.AddSingleton(strings));
        app.ConfigureServices(services => services.AddSingleton(integers));
        app.ConfigureModel<Model>(model => model.SetBinder(_ => new Model()));

        await app.RunAsync([]);
        
        strings.Single().ShouldBe(TestString);
        integers.Single().ShouldBe(TestInt);
    }
}