using Shouldly;
using Vertical.Cli.Conversion;
using Vertical.Cli.Engine;

namespace Vertical.Cli.Configuration;

public sealed class DefaultProviderTests
{
    public record Model(int Value);

    [Fact]
    public void Provides_Configured_Option_Default()
    {
        var command = new RootCommand<Model>("test");
        command.AddOption(x => x.Value, ["-v"], defaultProvider: () => 5);
        command.ConfigureOptions(opt => opt.ValueConverters.Add(new ParsableConverter<int>()));

        var context = CliEngine.GetContext(command, []);
        context.GetValue<int>(nameof(Model.Value)).ShouldBe(5);
    }
    
    [Fact]
    public void Preserves_Given_Option_Value()
    {
        var command = new RootCommand<Model>("test");
        command.AddOption(x => x.Value, ["-v"], defaultProvider: () => 5);
        command.ConfigureOptions(opt => opt.ValueConverters.Add(new ParsableConverter<int>()));

        var context = CliEngine.GetContext(command, ["-v=10"]);
        context.GetValue<int>(nameof(Model.Value)).ShouldBe(10);
    }
    
    [Fact]
    public void Provides_Configured_Argument_Default()
    {
        var command = new RootCommand<Model>("test");
        command.AddArgument(x => x.Value, defaultProvider: () => 5);
        command.ConfigureOptions(opt => opt.ValueConverters.Add(new ParsableConverter<int>()));

        var context = CliEngine.GetContext(command, []);
        context.GetValue<int>(nameof(Model.Value)).ShouldBe(5);
    }
    
    [Fact]
    public void Preserves_Given_Argument_Value()
    {
        var command = new RootCommand<Model>("test");
        command.AddArgument(x => x.Value, defaultProvider: () => 5);
        command.ConfigureOptions(opt => opt.ValueConverters.Add(new ParsableConverter<int>()));

        var context = CliEngine.GetContext(command, ["10"]);
        context.GetValue<int>(nameof(Model.Value)).ShouldBe(10);
    }
}