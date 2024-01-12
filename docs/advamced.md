# Vertical.Cli - Advanced Topics

## Model Binding

### Symbol matching behavior

The library will match symbols to model binding targets using the following behavior:
- Non alpha-numeric characters such as underscores, hyphens, and slashes are not compared
- If non alpha-numeric characters are encountered, the next alpha-numeric character is compared in a case-insensitive manner
- All other characters are match in a case-sensitive manner

The following list provides examples of comparison results:
- `source` matches `source` or `Source`
- `-a` matches `a` or `A`
- `--overwrite` matches `overwrite` or `Overwrite`
- `--no-overwrite` matches `noOverwrite` or `NoOverwrite`

The `[BindTo]` attribute can be used to explicitly bind a model member to a specific symbol. Only the primary identity of the symbol is matched in this case; aliases are not compared.

```csharp
public record Parameter([BindTo("--path")] IEnumerable<string> Paths);

var rootCommand = RootCommand.Create<int>(
    id: "progran",
    configure: root => root.AddOption<string>("--path", arity: Arity.ZeroOrMany));
```

### Custom binding

Application's can fully control binding by adding an `IBinder` instance to `CliOptions`. To bind a model manually, perform the following:

```csharp
// Decorate the model with the following attribute:
[ModelBinder<MyModelBinder>]
public record MyModel(string Color, string Size, string Shape);

// Define a binder
public sealed class MyModelBinder : ModelBinder<MyModel>
{
    public override MyModel BindInstance(IBindingContext bindingContext)
    {
        var color = bindingContext.GetValue<string>("--color");
        var size = bindingContext.GetValue<string>("--size");
        var shape = bindingContext.GetValue<string?>("--shape");

        return new MyModel(color, size, shape);
    }
}

// Add binder to options
var options = new CliOptions();

options.ModelBinders.Add(new MyModelBinder());

// Pass the options object when creating the root command
var rootCommand = RootCommand.Create<int>(
    id: "program",
    configure: root =>
    {
        root.AddOption<string>("--color", arity: Arity.One)
            .AddOption<string>("--size", arity: Arity.One)
            .AddOption<string?>("--shape", defaultProvider: () => "square");
    },
    options);
```

## Inspecting the call site

Application's can see all the data the library uses to perform command invocation by inspecting the `ICallSiteContext` object. The best way to see this is to view the generated source file. This data can be obtained by creating the call site manually.

```csharp
// Note: the default value is necessary for call sites that are invoked that do not
// delegate to a command handler. For instance, call sites created when the help
// formatter is invoked or an argument error needs to be displayed.
var callSiteContext = CallSiteContext.Create(rootCommand, args, defaultValue: 0);

// Inspect context properties...
```

The `CallSite` property of the context contains the following data:

|Property|Description|
|---|---|
|State|A `CallState` enum value, either `Command`, `Help`, or `Faulted`.|
|ModelType|The type of model that requires binding.|
|Subject|The command that needs to be invoked.|

The `BindingContext` contains a variety of data including the parsed arguments, symbols, and various services needed for binding.

|Property|Description|
|---|---|
|Subject|The command that needs to be invoked.|
|RawArguments|The original program arguments.|
|SubjectArguments|The argument applicable to the subject command.|
|ArgumentSyntax|An array of syntax objects that describe how arguments are prefixed, operand values, etc.|
|SemanticArguments|A collection of arguments that have a high level of semantic meaning.|
|ConverterDictionary|A dictionary of converters keyed by value type.|
|ValidatorDictionary|A dictionary of validators keyed by value type.|
|SymbolIdentities|The aggregated identities of all visible symbols in the command path.|
|ArgumentSymbols|The argument symbols visible in the command path.|
|OptionSymbols|The option symbols visible in the command path.|
|SwitchSymbols|The switch symbols visible in the command path.|
|HelpOptionSymbol|If defined, a reference to the help option symbol.|
|BindingDictionary|A dictionary of argument bindings, used to provide final values.|

