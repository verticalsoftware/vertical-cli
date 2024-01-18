using Vertical.Cli.Binding;
using Vertical.Cli.Validation;

namespace Vertical.Cli.Invocation;

[UsesVerify]
public class CallSiteCliExceptionTests
{
    public record Model(
        bool Switch,
        string Shape,
        string[] Colors,
        int Count,
        TimeSpan Interval);

    public static readonly ModelBinder<Model> ModelBinder = new DelegatedBinder<Model>(
        context => new Model(
            context.GetValue<bool>("--switch"),
            context.GetValue<string>("--shape"),
            context.GetValues<string>("--color").ToArray(),
            context.GetValue<int>("--count"),
            context.GetValue<TimeSpan>("--interval")));

    private static readonly IRootCommand<Model, int> Root = RootCommand
        .Create<Model, int>("root", cmd =>
        {
            cmd.SetHandler(_ => 0);

            cmd.AddSwitch("--switch");
            cmd.AddOption<string>("--shape", arity: Arity.One);
            cmd.AddOption<string>("--color", arity: new Arity(0, 3));
            cmd.AddOption<int>("--count");
            cmd.AddOption("--interval", validator: Validator
                .Configure<TimeSpan>(x => x.LessThan(TimeSpan.FromSeconds(30))));

        }, new Func<CliOptions>(() =>
        {
            var options = new CliOptions();
            options.AddBinder(() => ModelBinder);
            return options;
        })());

    [Fact]
    public Task ReturnsInvalidSwitchError()
    {
        var args = new[]
        {
            "--shape=square",
            "--switch=not_boolean"
        };

        return Verify(GetError(args));
    }

    [Fact]
    public Task ReturnsMinimumArityNotMetError()
    {
        return Verify(GetError(Array.Empty<string>()));
    }

    [Fact]
    public Task ReturnsArityExceededError()
    {
        var args = new[]
        {
            "--shape=square",
            "--color=red",
            "--color=green",
            "--color=blue",
            "--color=cyan"
        };

        return Verify(GetError(args));
    }

    [Fact]
    public Task ReturnsValueConversionError()
    {
        var args = new[]
        {
            "--shape=square",
            "--count=not_integer"
        };

        return Verify(GetError(args));
    }

    [Fact]
    public Task ReturnsValidationError()
    {
        var args = new[]
        {
            "--shape=square",
            "--interval=00:45:00"
        };

        return Verify(GetError(args));
    }

    [Fact]
    public Task ReturnsOptionMissingOperandError()
    {
        var args = new[]
        {
            "--shape"
        };

        return Verify(GetError(args));
    }

    private static string GetError(IEnumerable<string> args)
    {
        var options = new CliOptions();
        options.AddBinder(() => ModelBinder);
        var root = RootCommand
            .Create<Model, int>("root", cmd =>
            {
                cmd.SetHandler(_ => 0);

                cmd.AddSwitch("--switch");
                cmd.AddOption<string>("--shape", arity: Arity.One);
                cmd.AddOption<string>("--color", arity: new Arity(0, 3));
                cmd.AddOption<int>("--count");
                cmd.AddOption("--interval", validator: Validator
                    .Configure<TimeSpan>(x => x.LessThan(TimeSpan.FromSeconds(30))));

            }, options);

        var context = CallSiteContext.Create(root, args, 0);
        Assert.NotNull(context.BindingException);
        return context.BindingException!.Message;
    }
}