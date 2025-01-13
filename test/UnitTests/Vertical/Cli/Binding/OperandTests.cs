using System.Text.RegularExpressions;
using Shouldly;
using Vertical.Cli.Configuration;
using Vertical.Cli.Conversion;

namespace Vertical.Cli.Binding;

public class OperandTests
{
    public class KeyValuePairConverter : ValueConverter<KeyValuePair<string, string>>
    {
        /// <inheritdoc />
        public override KeyValuePair<string, string> Convert(string str)
        {
            if (Regex.Match(str, "([^=]+)=(.+)") is not { Success: true } match)
                throw new ArgumentException();

            return new KeyValuePair<string, string>(match.Groups[1].Value, match.Groups[2].Value);
        }
    }

    public record Options(KeyValuePair<string, string>[] Properties, string? LogLevel);
    
    [Fact]
    public void Bind_Argument_Text_Returns_Expected()
    {
        var app = new CliApplicationBuilder("test")
            .AddConverters([new KeyValuePairConverter()])
            .MapModel<Options>(map => map.Option(x => x.Properties, ["--watermark"], Arity.ZeroOrMany))
            .Route<Options>("test", opt => 0)
            .Build();

        var context = CliEngine.GetBindingContext(app, ["--watermark", "id=value"]);
        var props = context.GetArray<KeyValuePair<string, string>>(nameof(Options.Properties));
        
        props.Single().Key.ShouldBe("id");
        props.Single().Value.ShouldBe("value");
    }

    [Fact]
    public void Bind_Operand_Value_Returns_Expected()
    {
        var app = new CliApplicationBuilder("test")
            .AddConverters([new StringConverter()])
            .MapModel<Options>(map => map.Option(x => x.LogLevel, ["--level"]))
            .Route<Options>("test", opt => 0)
            .Build();

        CliEngine
            .GetBindingContext(app, ["--level:debug"])
            .GetValue<string?>(nameof(Options.LogLevel))
            .ShouldBe("debug");
        
        CliEngine
            .GetBindingContext(app, ["--level=debug"])
            .GetValue<string?>(nameof(Options.LogLevel))
            .ShouldBe("debug");
        
        CliEngine
            .GetBindingContext(app, ["--level", "debug"])
            .GetValue<string?>(nameof(Options.LogLevel))
            .ShouldBe("debug");
    }
}