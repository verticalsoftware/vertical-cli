# Application Services

## Overview

Command handler implementations can be resolved from a service provider if the application leverages dependency injection. The interface implementations must implement is `IHandler<TModel>` where `TModel` is the type of options object. The application then needs to supply function that returns an `IServiceProvider` instance. The executing context is supplied to the delegate.

## Example

```csharp
// Handler...
public class CompressHandler : IHandler<ICompressOptions>
{
    public async Task<int> HandleAsync(
        [GeneratedBinding] ICompressOptions options,
        CancellationToken cancellationToken)
    {
        // Do the work asynchronously
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

The service provider factory delegate is called just before invocation, and if configured, is disposed of after invocation.