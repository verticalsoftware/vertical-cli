# Application Data & Services

## Overview

If an application wants to leverage dependency injection, use the [vertical-cli-dependencyinjection](https://www.nuget.org/packages/vertical-cli/) package.

A class that implements `IHandler<TModel>` can be registered in the framework's service collection (provided as an extension property of `CommandLineApplication`).

## Example

```csharp
// Handler...
public class CompressHandler : IHandler<ICompressOptions>
{
    public async Task<int> HandleAsync(
        [GeneratedBinding] ICompressOptions options,
        CancellationToken cancellationToken)
    {
        // Do the work asynchronously, return an exit code
        return 0;
    }
}

// Setup...
var rootCommand = new RootCommand("compress");

// Set the handler to resolve from dependency injection
rootCommand.SetHandler<ICompressOptions, CompressHandler>();

var app = new CommandLineApplication(rootCommand);

// Register the type
app.Services.AddSingleton<CompressHandler>();

return await app.ConfigureAndRunAsync(args);
```

## Passing data within the framework

Sometimes it is necessary for application data to be available to various integration points of the framework. A simple options system is available in the configuration and then at certain integration points.

An application leverage this capability using a constructor-less class with writeable properties. The options API has three methods:

- `Confiure<TOptions>(Action<TOptions>)` - the framework provides a singleton instance of `TOptions` for access.
- `GetOptions<TOptions>()` - returns the singleton options instance.
- `Contains(Type)` - returns whether the options manager has already created an instance of the type.

Application data is available within the following integrations:
- Configuration using the `CommandLineApplication` instance
- Provided during [directive handling](./directives.md)
- Provided in the `InvocationContext` object during a [middleware](./middleware.md) hook
- Provided during property and model [binding](./binding.md)