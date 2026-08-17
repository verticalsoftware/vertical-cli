# Using services

### Overview

The [vertical-cli-dependencyinjection](https://www.nuget.org/packages/vertical-cli-dependencyinjection/) extends the functionality of the base library with dependency injection support. Several notable things are introduced by the package:

- The `CommandLineApplication` class gets an extension method named `ConfigureServices`, which registers a delegate that receives the invocation context and the service collection.
- The `Command` class gets two extension `SetHandlerService` methods that can be used to instruct the framework to resolve `IHandler<TModel>` implementations for commands from a service provider.
- The `InvocationContext` class gets a `BuildServiceProvider()` extension method. Application's don't need to use this unless they need the service provider in a middleware component.

### Using services for command handlers

An application can define a class that implements `IHandler<TModel>` to perform command functions. Using a class opens the instance to injection of application services. Using services for command functions requires the handler implementations to be registered in the service colection, and the command's handler set to resolve from the service provider. The following example illustrates this concept:

```csharp
// Define the model
interface IOptions
{
    /* Properties */
}

// Define a handling class
sealed class MyAppHandler(/* inject services */) : IHandler<IOptions>
{
    public async Task HandleAsync(IOptions options, CancellationToken cancellationToken)
    {
        // Perform work and return an exit code
        return 0;
    }
}

// Configuration
var rootCommand = new RootCommand("app");

// Set the handler to resolve from the service provider
rootCommand.SetHandlerService<IOptions, MyAppHandler>();

var app = new CommandLineApplication(rootCommand);

app.ConfigureSevices((context, services) => 
    {
        // Register the handler type
        services.AddSingleton<MyAppHandler>();
        
        // Register other application services...
        services.AddLogging();
    });

app.Configure();
return await app.RunAsync(args);
```

> ℹ️ Note
> 
> The framework manages the lifetime of its service provider.