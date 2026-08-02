# Implementing advanced model binding

## Overview

The framework performs the following to get a strongly typed options object to a command handler method:

- Parses the raw input strings provided by the application's entry point `args` into `ArgumentToken` values. The parser determines the structure of each argument, e.g. whether it has a symbol, an attached parameter value, etc.
- Composes a `ParseResult`, which associates each input token with the property of the model, performs conversions from `string` to the property type, and checks to see if arity requirements are met.
- Invokes the `ModelBinder<TModel>` delegate. This delegate receives a `BindingContext<TModel>` and is responsible for create instances of `TModel` using the parsed and converted values.

## Model Binding

Given a type, a model binding operation looks like the following:

```csharp
interface IOptions
{
    string UserId { get; }
    string Password { get; }
    int TimeoutSeconds { get; }
    bool RequireSsl { get; }
}

class OptionsImpl : IOptions
{
    // ...
}

// Model binding
IOptions Bind(BindingContext<IOptions> context)
{
    return new OptionsImpl
    {
        UserId = context.GetValue(x => x.UserId),
        Password = context.GetValue(x => x.Password),
        TimeoutSeconds = context.GetValue(x => x.TimeoutSeconds),
        RequireSsl = context.GetValue(x => x.RequireSsl)
    };
}
```

The source generator creates all of this repetitive code as well as generating an internal implementation for the interface. It is signaled with the `GeneratedBinding` attribute, which can be placed on an interface or on the parameter of the command handler.

```csharp
[GeneratedBinding]
interface IOptions
{
    // ...
}

// alternatively in the command handler
async Task<int> HandleAsync([GeneratedBinding] IOptions options, CancellationToken token)
{
    return Task.FromResult(0);
}
```

In the event an interface cannot be used as a model type, the application becomes responsible for binding. It can register an action while configuring the parser.

```csharp
app.ConfigureParser<OptionsImpl>(builder => 
    builder
        .SetBinder(context => new OptionsImpl
        {
            // Set properties
        }));
```

## Configuring private bindings

When applications need option objects constructed with data that is not specified with user input, it configures a private binding. The parser can be configured with application data that the user is not aware of.

The following example shows different ways private bindings can be configured:

```csharp
app.ConfigureParser<IOptions>(builder =>
{
    // Assign a known value to a property during creation
    builder.MapStaticValue(x => x.Value, "value");

    // Assign an options value
    builder.MapBindingInfoValue(
        x => x.LogLevel,
        bindingInfo => bindingInfo.GetOptions<LoggerOptions>().LogLevel);

    // Assign the input stream
    builder.MapBindingInfoValue(
        x => x.InputTextReader,
        bindingInfo => bindingInfo.ConsoleInput)        ;
});
```

## Using custom application types

If an application has control over a model's type, then the most direct way to provide conversion support is to implement `IParsable<T>`. The source generator will detect this when inspecting the property type, and a converter will be made automatically available.

Otherwise, a `Converter<string, TValue>` must be provided. When implemnting an argument converter, throw an exception if the conversion fails. Only include a short description of the problem - the parser will compose a message with the appropriate identifier.

Similarly, if a custom collection type is used that is not one of the BCL types, a `Converter<IEnumerable<TValue>, TCollection> where TCollection : IEnumerable<TValue>` must be provided.

The following example demonstrates how `Dictionary<TKey, TValue>` collections can be used in a command's model. This involves creating both an argument and a collection converter. This is required because `KeyValuePair<>` is not `IParsable<T>`.

```csharp
// Convert string arguments to KeyValuePair
app.AddArgumentConverter(str =>
{
    if (Regex.Match(str, @"(\w+):(.+)") is not { Success: true } match)
    {
        throw new ArgumentException("invalid key/value pair.");
    }

    return new KeyValuePair<string, string>(
        match.Groups[1].Value,
        match.Groups[2].Value
    );
});

// Convert KeyValuePairs to Dictionary
app.AddCollectionConverter(
    (IEnumerable<KeyValuePair<string, string>> values) =>
        new Dictionary<string, string>(values));
)

app.ConfigureParser<IOptions>(builder => builder
    .MapMultiValuedArgument(
        x => x.ConnectionProperties,
        ordinalPosition: 0,
        arity: Arity.OneOrMore
    ));

interface IOptions
{
    Dictionary<string, string> ConnectionProperties { get; }
}
```