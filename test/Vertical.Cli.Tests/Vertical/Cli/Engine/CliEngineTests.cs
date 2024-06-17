using Shouldly;
using Vertical.Cli.Configuration;
using Vertical.Cli.Conversion;
using Vertical.Cli.Internal;

namespace Vertical.Cli.Engine;

public class CliEngineTests
{
    enum Verbosity { Verbose, Info, Minimal }

    class BaseModel
    {
        public BaseModel(Verbosity verbosity)
        {
            Verbosity = verbosity;
        }

        public Verbosity Verbosity { get; }
    }

    class ConnectModel(Verbosity verbosity) : BaseModel(verbosity)
    {
        public Uri Host { get; init; } = default!;
        public uint Port { get; init; }
        public string UserId { get; init; } = default!;
        public string? Password { get; init; } = default!;
    }

    private readonly RootCommand<BaseModel, Task<int>> _root = Unit.Create(() =>
    {
        var root = new RootCommand<BaseModel, Task<int>>("db");

        root.AddOption(x => x.Verbosity,
            ["--verbosity"],
            defaultProvider: () => Verbosity.Minimal,
            scope: CliScope.Descendants);

        var connect = new SubCommand<ConnectModel, Task<int>>("connect");

        connect
            .AddOption(x => x.Host, ["-h", "--host"], Arity.One)
            .AddOption(x => x.Port, ["--port"], defaultProvider: () => (uint)3306)
            .AddOption(x => x.UserId, ["-u", "--user"], Arity.One)
            .AddOption(x => x.Password, ["-p", "--password"])
            .SetHandler((model, cancel) => Task.FromResult(1));

        root.AddSubCommand(connect);

        return root;
    });

    [Fact]
    public async Task InvokesSubCommand()
    {
        var result = await InvokeAsync(_root, [
            "connect",
            "-h=https://mysql.azure.net",
            "-u", "tester",
            "-p", "(secret)"
        ]);
        
        result.ShouldBe(1);
    }

    [Fact]
    public async Task Throws_When_Arity_Not_Met()
    {
        await Should.ThrowAsync<CommandLineException>(async () =>
            await InvokeAsync(_root, [
                "connect",
                "-h=https://mysql.azure.net",
                "-p", "(secret)"
            ]));
    }
    
    [Fact]
    public async Task Throws_When_Conversion_Fails()
    {
        await Should.ThrowAsync<CommandLineException>(async () =>
            await InvokeAsync(_root, [
                "connect",
                "-h=https://mysql.azure.net",
                "--port", "(invalid)"
            ]));
    }

    [Fact]
    public async Task Throws_For_Unknown_Args()
    {
        await Should.ThrowAsync<CommandLineException>(async () =>
            await InvokeAsync(_root, [
                "connect",
                "-h=https://mysql.azure.net",
                "-u", "test",
                "-p", "(secret)",
                "-z=unknown"
            ]));
    }

    private static async Task<int> InvokeAsync(RootCommand<BaseModel, Task<int>> command, string[] args)
    {
        // This is what generator would make
        var context = CliEngine.GetContext(command, args);
        var modelType = context.ModelType;
        var converters = command.Options.ValueConverters;

        if (modelType == typeof(ConnectModel))
        {
            converters.Add(UriConverter.Default);
            converters.Add(new ParsableConverter<uint>());
            converters.Add(new EnumConverter<Verbosity>());
            converters.Add(StringConverter.Default);

            var model1 = new ConnectModel(context.GetValue<Verbosity>("Verbosity"))
            {
                Host = context.GetValue<Uri>("Host"),
                Port = context.GetValue<uint>("Port"),
                UserId = context.GetValue<string>("UserId"),
                Password = context.GetValue<string>("Password")
            };

            context.AssertBinding(model1);

            var callSite1 = context.GetCallSite<ConnectModel>();
            return await callSite1(model1, CancellationToken.None);
        }
        
        throw Exceptions.InvocationFailed(command, args);
    }
}