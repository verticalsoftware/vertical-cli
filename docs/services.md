# Application Data & Services

## Overview

Command handler implementations can be resolved from a service provider if the application leverages dependency injection. The handler class must implement the `IHandler<TModel>` interface where `TModel` is the type of options object. The application then needs to supply a function that returns an `IServiceProvider` instance. The executing context is supplied to the delegate.

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
var serviceCollection = new ServiceCollection();
serviceCollection.AddSingleton<CompressHandler>();

var rootCommand = new RootCommand("compress");
rootCommand.SetHandler<CompressHandler>();

var app = new CommandLineApplication(rootCommand);
app.UseServices(context => serviceCollection.BuildServiceProvider(), dispose: true);

return await app.ConfigureAndRunAsync(args);
```

## Lifetime of the service provider

The `UseServices()` method has an optional boolean parameter `dispose`. When set `true`, the framework will try to dispose of the `IServiceProvider` instance, otherwise it remains the application's responsibility.

The service provider factory delegate is called just before invocation of the handler mthod, and if configured, is disposed of immediately after the handling method returns.

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