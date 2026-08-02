# Middleware

## Overview

Prior to parsing, binding, and passing control flow to a command handler, the framework routes the tokens through a middleware pipeline. This enables features like responding to directives, calling the help system, etc. The pipeline is a connected set of components that follow the _chain of responsibility_ pattern, where each component performs its work and passes control to the next component (not necessarily in that order).

## The default setup

The following table list the components in the default pipeline:

|Component|Description|
|---|---|
|Directives handler|Matches directives in the input token list and invokes the event handler configured by the application.|
|Help System|Scans the input token for the help option (by default `--help` or `-?`). When matched, displays the appropriate help article and extis the pipeline.|
|Automatic command help|Runs the forward pipeline and detects whether a command was matched. When one is not matched, it automatically displays the appropriate help article and exists the pipeline.|
|Display input errors|Runs the forward pipeline and detects whether there are errors with the user's input. If errors are detected, it displays them to the console.|
|Inject response arguments|Scans tokens for response file annotations. When matched, the strings in the response file are parsed and injected into token list.|
|Handle cancellation|Listens for `SIGTERM`/`SIGINT`. When detected, it will send a cancellation signal on the context's `CancellationTokenSource`.

## Writing a middleware component

Middleware components are asynchronous methods with the following signature:

```csharp
async Task InvokeAsync(InvocationContext context, Func<InvocationContext, Task> next)
{    
    // context: Contains the token list and a mostly read-only view of the configuration
    // next: Called to pass control to the next component
}
```

Common implementation patterns include:
- Perform the middleware work, then call `await next(context)`.
- Pass control to the rest of the pipeline first with `await next(context)`, then evaluate the state of the context and perform post application work.
- Detect a state in the context, perform any work, and then short circuit.


## Configuring the pipeline

Use the `CommandLineApplication.ConfigureMiddleware` method to configure the pipeline. The method registers a delegate that manipulates the middleware builder. The builder API lets applications add components before the start or after the end of the default pipeline, or completely build the pipeline from scratch.

The following example adds a top level middleware that catches and prints exceptions thrown by the application:

```csharp
app.ConfigureMiddleware(builder => builder.AddFirst(
    async (context, next) => 
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            context.OutputWriter.WriteLine(
                exception.ToString(),
                DisplayElement.Important
            );
        }
    }
));
```

## The invocation context

The invocation context object provided in middleware provides the following notable data:

|Property|Description|
|---|---|
|`TokenList`|The mutable list of parsed user input tokens|
|`Arguments`|The original arguments provided by the application (entry point `args`).|
|`Errors`|A list of `CommandLineError` objects that provide a context of user input errors.|
|`OutputWriter`|An object that formats output to the console abstraction.|
|`ApplicationOptions`|The options manager that maintains singleton application objects that are shared in the pipeline and with certain integration hooks.|