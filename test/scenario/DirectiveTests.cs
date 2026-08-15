using Shouldly;
using Vertical.Cli.Configuration;
using Vertical.Cli.Conversion;
using Vertical.Cli.ScenarioTests.Common;

namespace Vertical.Cli.ScenarioTests;

public class DirectiveTests
{
    [Fact]
    public async Task CallDirective_Invokes_Handler()
    {
        var command = new RootCommand("app");
        command.SetHandler<object>((_, _) => Task.FromResult(0));

        var app = new CommandLineApplication(command);
        app.ConfigureModel<object>(builder => builder.SetBinder(_ => new object()));
        app.HandleDirective(
            "test",
            eventInfo =>
            {
                eventInfo.Context.OutputWriter.WriteLine("directive invoked");
                return Task.CompletedTask;
            });

        var result = await TestApplicationFixture.GetOutputAsync(app, ["[test]"]);
        result.ShouldStartWith("directive invoked");
    }

    [Fact]
    public async Task CallParameterizedDirective_Invokes_Handler()
    {
        var command = new RootCommand("app");
        command.SetHandler<object>((_, _) => Task.FromResult(0));

        var app = new CommandLineApplication(command);
        app.ConfigureModel<object>(builder => builder.SetBinder(_ => new object()));
        app.HandleParameterizedDirective<string>(
            "test",
            eventInfo =>
            {
                eventInfo.Context.OutputWriter.WriteLine($"directive invoked with arg '{eventInfo.Value}'");
                return Task.CompletedTask;
            });

        app.AddArgumentConverter(Converters.Default);

        var result = await TestApplicationFixture.GetOutputAsync(app, ["[test:value]"]);
        result.ShouldStartWith("directive invoked with arg 'value'");
    }
}