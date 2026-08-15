# Commands

## Overview

Commands represent functions of the application. A `vertical-cli` application must have a _root_ command, but a hierarchy of sub commands can be defined.

### Sub commands

The .NET CLI utility `dotnet nuget push` is an example of a command hierarchy. `dotnet` can be thought of as the root command, `nuget` is its sub command, and `push` is a sub command of `nuget`. Commands can implement a handler method and/or define sub commands. If it does neither, it's considered  a dead-end and the framework will throw a configuration exception. 

The following example minimally mocks `dotnet nuget push`:

```csharp
var rootCommand = new RootCommand("dotnet");

var nugetCommand = new SubCommand("nuget");
rootCommand.AddSubCommand(nugetCommand);

var pushCommand = new SubCommand("push");
pushCommand.SetHandler<INugetPushOptions>(async (options, cancellationToken) =>
{
   // Push to nuget
   await NugetApi.Push(...);
   return 0;
});
```

### Handler methods

If the command performs a function, it needs to define a handler implementation. Internally, a handler is represented by the `IHandler<TModel>` interface. Application's can use a delegate or an instance of `IHandler<TModel>`.

A handler method receives a model instance  with a `CancellationToken`, performs work, and returns the exit code.

The following example sets up a delegate to perform the application's function.

```csharp
// Use a delegate
command.SetHandler<IOptions>(async (options, cancellationToken) => 
{
    // Perform asynchronous work. options contains arguments as a strong type
    return 0;
});
```

This example assumes the application defined an implementation of `IHandler<TModel>`. An instance of the handler type is created by the application and provided by a delegate. The `context` parameter shown is the framework's runtime context and is discussed in another article.

```csharp
command.SetHandler(context => new MyCommandHandler());
```

The final `SetHandler` overload wraps a handler in an `IAsyncDisposable` type. This can be useful if the application needs to manage resources. This overload is also used by the dependency injection package to manage the service provider. The example shows how an application can store data and then dispose of it when the lifecycle is over.

```csharp
// Define a provider
sealed class MyHandlerProvider(Func<IHandler<IOptions>> factory, IDisposable resource) 
    : HandlerServiceProvider<IOptions>(factory)
{
    public override async ValueTask DisposeAsync()
    {
        resource.Dispose();
    }
}

// In setup
command.SetHandler(context => new MyHandlerProvider(() => new MyCommandHandler(), disposableResource));
```

> ℹ️ Note
> 
> Factory delegates are used because the framework will never create instances of things it doesn't need. When an application runs and arguments are parsed, only a single command is ever invoked, therefore the framework only asks for an instance of the target command and nothing else.