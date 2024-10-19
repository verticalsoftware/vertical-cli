using Shouldly;
using Vertical.Cli.Conversion;
using Vertical.Cli.Engine;
using Vertical.Cli.Parsing;

namespace Vertical.Cli.Configuration;

public class ArgumentTransformationTests
{
    public class RootModel {}

    public class SubModel : RootModel
    {
        public int Count { get; set; }
    }
    
    [Fact]
    public async Task Engine_Calls_Transformers()
    {
        SubModel? subModel = null;
        var rootCommand = new RootCommand<RootModel>("root");
        var subCommand = rootCommand.AddSubCommand<SubModel>("counter");
        subCommand.AddArgument(x => x.Count);
        subCommand.Handle(opt => subModel = opt);

        rootCommand.ConfigureOptions(options =>
        {
            options.ArgumentTransforms.Add(list => list.FirstOrDefault()?.Text == "alias"
                ?
                [
                    ArgumentSyntax.Parse("counter"),
                    ArgumentSyntax.Parse("5")
                ]
                : list);
        });

        var context = CliEngine.GetContext(rootCommand, ["alias"]);
        if (context.TryGetCallSite<SubModel>(out var callsite))
        {
            rootCommand.Options.ValueConverters.Add(new ParsableConverter<int>());
            
            var model = new SubModel
            {
                Count = context.GetValue<int>(nameof(SubModel.Count))
            };
            var result = await callsite(model, CancellationToken.None);
            result.ShouldBe(0);
        }

        subModel!.Count.ShouldBe(5);
    }
}