using NSubstitute;
using Vertical.Cli.Binding;
using Vertical.Cli.Validation;

namespace Vertical.Cli.Configuration;

[UsesVerify]
public class ConfigurationValidatorTests
{
    public class DefaultBinder<T> : ModelBinder<T>
    {
        /// <inheritdoc />
        public override T BindInstance(IBindingContext bindingContext) => default!;
    }
    
    [Fact]
    public Task ReturnsExpectedWhenConverterMissing()
    {
        var rootCommand = RootCommand.Create<int>("root", root =>
        {
            root.AddArgument<IntPtr>("arg");
            root.SetHandler(_ => 0);
        });
        
        return Verify(rootCommand.GetErrors());
    }

    [Fact]
    public Task ReturnsExpectedWithDuplicateConverters()
    {
        var options = new CliOptions();
        options.AddConverter<DateTime>();
        options.AddConverter<DateTime>();

        var rootCommand = RootCommand.Create<int>("root", root =>
        {
            root.SetHandler(_ => 0);
        }, options);

        return Verify(rootCommand.GetErrors());
    }

    [Fact]
    public Task ReturnsExpectedWithDuplicateValidators()
    {
        var options = new CliOptions();
        options.AddValidator(Validator.Configure<string>(x => x.MinimumLength(1)));
        options.AddValidator(Validator.Configure<string>(x => x.MinimumLength(1)));
        options.AddValidator(Substitute.For<Validator<string>>());

        var rootCommand = RootCommand.Create<int>("root", _ => { }, options);

        return Verify(rootCommand.GetErrors());
    }

    [Fact]
    public Task ReturnsExpectedWhenMultiValueArgumentOutOfOrder()
    {
        var rootCommand = RootCommand.Create<int>("root", root =>
        {
            root.SetHandler(_ => 0);
            root.AddArgument<string>("arg1", Arity.ZeroOrMany);
            root.AddArgument<string>("arg2", Arity.One);
        });

        return Verify(rootCommand.GetErrors());
    }
    
    [Fact]
    public Task ReturnsExpectedWhenMultiValueArgumentOutOfOrderInPath()
    {
        var rootCommand = RootCommand.Create<int>("root", root =>
        {
            root.SetHandler(_ => 0);
            root.AddArgument<string>("arg1", Arity.ZeroOrMany, scope: SymbolScope.Descendents);
            root.ConfigureSubCommand("cmd", cmd =>
            {
                cmd.AddArgument<string>("arg2", Arity.One);
                cmd.SetHandler(_ => 0); 
            });
        });

        return Verify(rootCommand.GetErrors());
    }

    [Fact]
    public Task ReturnsExpectedWhenMultiValueArgumentsConfigured()
    {
        var rootCommand = RootCommand.Create<int>("root", root =>
        {
            root.SetHandler(_ => 0);
            root.AddArgument<string>("arg1", Arity.ZeroOrMany);
            root.AddArgument<string>("arg2", Arity.ZeroOrMany);
        });

        return Verify(rootCommand.GetErrors());
    }
    
    [Fact]
    public Task ReturnsExpectedWhenMultiValueArgumentsConfiguredInPath()
    {
        var rootCommand = RootCommand.Create<int>("root", root =>
        {
            root.SetHandler(_ => 0);
            root.AddArgument<string>("arg1", Arity.ZeroOrMany, scope: SymbolScope.Descendents);
            root.ConfigureSubCommand("cmd", cmd =>
            {
                cmd.AddArgument<string>("arg2", Arity.ZeroOrMany);
                cmd.SetHandler(_ => 0);
            });
        });

        return Verify(rootCommand.GetErrors());
    }

    [ModelBinder<DefaultBinder<EmptyModel>>]
    public record EmptyModel;

    [Fact]
    public Task ReturnsExpectedWhenMissingCustomBinder()
    {
        var rootCommand = RootCommand.Create<EmptyModel, int>("root",
            root => root.SetHandler(_ => 0));

        return Verify(rootCommand.GetErrors());
    }

    [Fact]
    public Task ReturnsExpectedWhenMissingBindingsForSymbols()
    {
        var options = new CliOptions();
        options.AddBinder(() => new DefaultBinder<EmptyModel>());

        var rootCommand = RootCommand.Create<EmptyModel, int>("root",
            root =>
            {
                root.AddArgument<string>("argument");
                root.SetHandler(_ => 0);
            },
            options);

        return Verify(rootCommand.GetErrors());
    }

    [ModelBinder<DefaultBinder<BoundModel>>]
    private record BoundModel(string SingleArg, string[] ArrayArg);

    [Fact]
    public Task ReturnsExpectedWhenBindingIsIncompatible()
    {
        var options = new CliOptions();
        options.AddBinder(() => new DefaultBinder<BoundModel>());

        var rootCommand = RootCommand.Create<BoundModel, int>("root",
            root =>
            {
                root.SetHandler(_ => 0);
                root.AddArgument<int>("singleArg");
            },
            options);

        return Verify(rootCommand.GetErrors());
    }

    private record IncompatibleCollectionModel(string Argument);

    [Fact]
    public Task ReturnsExpectedWhenBindingIsNotCollectionType()
    {
        var rootCommand = RootCommand.Create<IncompatibleCollectionModel, int>("root",
            root =>
            {
                root.SetHandler(_ => 0);
                root.AddArgument<string>("argument", arity: Arity.ZeroOrMany);
            });

        return Verify(rootCommand.GetErrors());
    }

    private record MismatchedBindingIdModelType([BindTo("--unknown")] bool Switch);

    [Fact]
    public Task ReturnsExpectedWhenBindToAttributeDoesntMatchSymbol()
    {
        var rootCommand = RootCommand.Create<MismatchedBindingIdModelType, int>(
            "root",
            root =>
            {
                root.AddSwitch("--switch");
                root.SetHandler(_ => 0);
            });

        return Verify(rootCommand.GetErrors());
    }

    private record IncompatibleCollectionModelType(string[] Arguments);

    [Fact]
    public Task ReturnsExpectedWHenBindingIsNotCompatibleValueType()
    {
        var rootCommand = RootCommand.Create<IncompatibleCollectionModelType, int>(
            "root",
            root =>
            {
                root.AddArgument<decimal>("Arguments", arity: Arity.ZeroOrMany);
                root.SetHandler(_ => 0);
            });

        return Verify(rootCommand.GetErrors());
    }

    public class ModelWithPrivateConstructor
    {
        private ModelWithPrivateConstructor()
        {
        }
    }

    [Fact]
    public Task ReturnsExpectedWhenModelHasNoConstructors()
    {
        var rootCommand = RootCommand.Create<ModelWithPrivateConstructor, int>("root",
            _ => { });

        return Verify(rootCommand.GetErrors());
    }

    public class ModelWithMultipleConstructors
    {
        public ModelWithMultipleConstructors(int _) { }
        public ModelWithMultipleConstructors(bool _) { }
    }

    [Fact]
    public Task ReturnsExpectedWhenModelHasMultipleConstructors()
    {
        var rootCommand = RootCommand.Create<ModelWithMultipleConstructors, int>("root",
            _ => { });

        return Verify(rootCommand.GetErrors());
    }

    public record SingleArgModel(bool Switch);

    [Fact]
    public Task ReturnsExpectedWhenRequiredHandlerIsMissing()
    {
        var rootCommand = RootCommand.Create<SingleArgModel, int>("root",
            root =>
            {
                root.AddSwitch("--switch");
            });

        return Verify(rootCommand.GetErrors());
    }

    [Fact]
    public Task ReturnsExpectedWhenNonUniqueSymbolsInPath()
    {
        var rootCommand = RootCommand.Create<SingleArgModel, int>("root",
            root =>
            {
                root.AddSwitch("--switch", scope: SymbolScope.Descendents);
                root.ConfigureSubCommand("sub", cmd =>
                {
                    cmd.AddSwitch("--switch");
                    cmd.SetHandler(_ => 0);
                });
            });
        
        return Verify(rootCommand.GetErrors());
    }

    [Fact]
    public Task ReturnsExpectedWithDuplicateSubCommands()
    {
        var rootCommand = RootCommand.Create<int>("root",
            root =>
            {
                root.ConfigureSubCommand("child", _ => { });
                root.ConfigureSubCommand("child", _ => { });
            });

        return Verify(rootCommand.GetErrors());
    }

    [Fact]
    public Task ReturnsExpectedWithInvalidSymbolIdentities()
    {
        var rootCommand = RootCommand.Create<int>("root",
            root =>
            {
                root.SetHandler(_ => 0);
                root.AddSwitch("*invalid");
            });

        return Verify(rootCommand.GetErrors());
    }

    [Fact]
    public Task ReturnsExpectedWithInvalidCommandIdentities()
    {
        var rootCommand = RootCommand.Create<int>("root",
            root =>
            {
                root.ConfigureSubCommand("*invalid", _ => { });
            });

        return Verify(rootCommand.GetErrors());
    }

    [Fact]
    public void ThrowsWhenHasErrors()
    {
        var rootCommand = RootCommand.Create<int>("root",
            root =>
            {
                root.ConfigureSubCommand("*invalid", _ => { });
            });

        Assert.Throws<InvalidOperationException>(() => rootCommand.ThrowIfInvalid());
    }

    [Fact]
    public void DoesNotThrowWhenHasNoErrors()
    {
        var rootCommand = RootCommand.Create<int>("root", root =>
        {
            root.SetHandler(_ => 0);
        });
        
        rootCommand.ThrowIfInvalid();
    }
}