# Advanced configuration

This section covers more advanced setup topics.

## Additional symbol configuration

### Providing default values

Symbols can be configured with a default value. The behavior will apply if the application's user does not specify the option or argument. The following example configures a default value:

```csharp
builder.ConfigureModel<IArchivingOptions>(model => model
    .AddOption(
        propertyExpression: x => x.Compression,
        aliases: ["-c", "--compression"],
        setBindingOptions: binder => binder.UseDefaultValue(
            CompressionType.GZip));
```

### Binding static values

A model's property can be configured with a static binding value, meaning it does not have a symbol association. This can be useful if the application needs to pass internal data through the framework that the user is not aware of. This data is set in the model instance and available to commands. The following example passes a service provider:

```csharp
// Define a model to expose the static value property
interface IApplicationServices
{
    IServiceProvider Services { get; }
}

// A command model implements the interface
interface ICommandOptions : IApplicationServices
{
    // Other command option properties
}

// Configuration (Program.cs)
var services = new ServiceCollection()
    .BuildServiceProvider();

var builder = new CommandLineBuilder(
    new RootCommand<ICommandOptions>("app"));

builder.ConfigureModel<IApplicationServices>(model => model
    .BindStaticValue(
        propertyExpression: x => x.Services,
        setValue: () => services));
```

### Binding pending tokens

Pending tokens are command line arguments that cannot be matched to a defined symbol. If the application is interested in these tokens, they can be mapped to either a `string` collection using the `BindPendingTokenValues` method or a `Token` collection using the `BindPendingTokens` method. The receiving property must be a collection type that satisfies the `new()` type constraint.

```csharp
interace IPendingTokens
{
    List<string> PendingTokens { get; }
}

// configuration...

builder.ConfigureModel<IPendingTokens>(model => model.
    BindPendingTokenValues(x => x.PendingTokens));
```

By default, the parser will generate an error for each pending token, but this can be disabled by configuration:

```csharp
buider.ConfigureOptions(options => options.IgnorePendingTokens = true);
```

### Binding standard input

It's common practice to use the shell to redirect output from one program to the input of another. An example of this is the `grep` utility that seeks pattern matches in files, e.g. `cat myfile.txt | grep <pattern>`. Applications can bind the input stream by defining a `TextReader` property and calling the `BindStandardInput` method.

```csharp
interface IOptions
{
    TextReader Input { get; }
}

// configuration...
builder.ConfigureModel<IOptions>(model => model
    .BindStandardInput(x => x.Input));)
```

> 💡 Note
>
> The framework abstracts console operations using the `IConsole` interface. The default implementation simply wraps `System.Console`, but applications can provide their own implementation.

### Manually binding a property

In rare cases, the application may need to manually provide a binding value for a property. This is accomplished by defining a binding implementation.

The following example binds a dictionary which is not supported by the framework out-of-box. The implementation splits string into key/value pairs using a multi-valued arity symbol.

```csharp
interface IOptions
{
    Dictionary<string, string> Tags { get; }
}

// configuration...
builder.ConfigureModel<IOptions>(model => model
    .AddOption(
        propertyExpression: x => x.Tags,
        aliases: ["--tag"],
        setBindingOptions: binder =>
        {
            // Get the string arguments that match the tag
            var values = binder.GetValues();

            // Create the dictionary
            var entries = values
                .Select(str =>
                {
                   var split = str.Split('=');
                   return new KeyValuePair<string, string>(
                        split[0],
                        split[1]
                   );
                });

            var dictionary = new Dictionary<string, string>(entries);

            binder.SetValue(dictionary);
        }
    ));
```

### Providing a model activator

If a class with a parameterized constructor is used and the application wants the source generator to supply the binding, then an activator must be provided. The activator is simply a `Func<TModel>` that creates the instance. In this case the source generator will request the new instance and then bind properties with a set accessor. Note `init` properties cannot be set by the generator because it will not have access during activation.

```csharp
builder.ConfigureModel<ParameterizedClass>(model => model
    .UseActivator(() => new ParameterizedClass(/* args */)));
```

### Manually binding a model

For complex use cases, the application cannot use the source generator. An example of this is when the model has a complex structure. Although this use case is supported by the `ModelConfiguration<TModel>` api, its usage is not recommended (the model design should be reconsidered).

The best way to see how a model can be bound manually is to refer to the demo application and the source generated code. Notably, the source generator makes use of the `BindingContext<TModel>` class, which is a higher level service that sits above the `ParseResult`. Both components are briefly explained below:

|Class|Description|
|---|---|
|`ParseResult`|Contains a mapping of string token values (derived from arguments) to their matched symbols. Values are retrieved by providing the property name.|
|`BindingContext<TModel>`|Sits overtop a `ParseResult` and is contextual to a specific model type. Strongly typed, converted values can be retrieved by providing the property expression.

### Providing value converters

If a model defines a property whose type is not supported by an intrinsic converter, an application can provide the conversion function. The following to approaches can be used:

- If the application controls the type, then the recommended strategy is to implement `IParsable<T>`.

- Define a `ValueConverter<T>` and register it using the `CommandLineBuilder` API.

The following example registers a value converter:

```csharp
public readonly record struct Coordinate(double Lat, double Long);

// configuration..
builder.AddValueConverter(str => 
{
    if (Regex.Match(
        str, 
        @"(-?[0-9]+(\.[0-9]+)?),(-?[0-9]+(\.[0-9]+)?)")
        is not { Success: true } match)
        {
            throw new FormatException("Invalid coordinate");
        }

        var lat = double.Parse(match.Groups[1].Value);
        var long = double.Parse(match.Groups[3].Value);
        return new Coordinate(lat, long);
});
```

## Up next

[Middleware](./middleware.md)