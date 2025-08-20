# Middleware

The framework uses a middleware pipeline to allow the application to interact with the argument tokens and the result. The pipeline implements the chain of responsibility pattern, e.g. middleware passes control to the next middleware or short circuits. 

### The invocation context

The state object passed through the middleware pipeline is an instance of the `InvocationContext` class. The following table summarizes the properties available in the context:

```csharp
public sealed class InvocationContext
{
    // Root command passed to the CommandLineBuilder
    public IRootCommand RootCommand { get; }

    /// Cancellation token source
    public CancellationTokenSource CancellationSource { get; }

    // Cancellation token provided to command handlers
    public CancellationToken CancellationToken { get; }
    
    // The parser service
    public IParser Parser { get; }
    
    // Mutable token list
    public LinkedList<Token> TokenList { get; }
    
    // Root configuration
    public IRootConfiguration Configuration { get; }
    
    // Array of original string arguments
    public string[] Arguments { get; }
    
    // Original arguments in token form
    public Token[] OriginalTokens { get; }
    
    // The console abstraction
    public IConsole Console { get; }

    // Usage errors generated in middleware
    public List<UsageError> Errors { get; } = [];
}
```

### Adding middleware

Custom middleware is added to the end of the pipeline by calling the `Add` method using the `MiddlewareConfiguration` API. The middleware component is a delegate of the following form:

```csharp
public delegate Task Middleware(
    InvocationContext context,
    Func<InvocationContext, Task> next
);
```

A middleware function can:
- Manipulate the token list
- Signal cancellation
- Generate errors
- Set the application exit code
- Invoke the next middleware or short-circuit the pipeline

### The default pipeline

By default, the middelware pipeline contains only a terminal component that routes control to the command handling function with a composed model instance. Other components defined by the framework include:

|Middleware|Description|
|---|---|
|Handle exceptions|The next middleware is invoked in a try/catch block. If an exception is thrown, it is printed to the console.|
|Help on error|The next middleware is invoked. If one or more usage errors were generated, contextual help is printed to the console.|
|Handle errors|The next middleware is invoked. If one or more usage errors were generated, they are printed to the console.|
|Handle cancellation|A callback is registered to the console abstraction to listen for `SIGTERM`. The callback signals the context's `CancellationTokenSource`.
|Response files|Adds a directive handler that injects tokens to the token stream read from response files.|
|Add help|Adds a middleware symbol that displays help content when one of the configured aliases are matched (defaults to [`--help`, `-?`])|
|Add version|Adds a middleware symbol that displays the applications version when one of the configured aliases are matched (defaults to `--version`)|

To use these components, call the `UseDefaults` method in the `MiddlewareConfiguration` API.

### Removing or replacing default middleware

After an application adds the default middleware, it can remove or replace individual components using the `Remove` or `Replace` methods in the `MiddlewareConfiguration` API. These methods accept a `BuiltInMiddleware` value that identifies the component to reomve or replace.

### Directives

Directives are special tokens that are enclosed in square brackets. They allow application's to implement cross cutting concerns that may not necessarily be connected to its functionality. One example is the response directive. This directive allows the user to specify paths to files that contain argument tokens to inject into the token list. Applications can implement their own directives by configuring them in the `MiddlewareConfiguration` API.

> 💡 Note
> 
> Directives must be the first tokens parsed in the argument list. When the first argument not matching the directive syntax is encountered, parsing of directives is disabled for the remainder of the arguments.

The following example defines a directive that allows the user to configure output logging.

```csharp
var services = new ServiceCollection();

var builder = new CommandLineBuilder(new RootCommand("app"));

builder.AddDirectiveHandler(
    // Application data can be provided to the directive handler
    services,
    // The context object contains the current token to evaluate
    async context =>
    {
        var syntax = context.Token.GetDirectiveSyntax();

        if (!Enum.TryParse(
            syntax.ParameterSpan,
            ignoreCase: true, 
            out LogLevel logLevel))
        {
            context.AddError($"Invalid log level '{tokenText}'");
            return;
        }

        context.State.AddLogging(builder => builder
            .SetMinimumLevel(logLevel));

        await Task.CompletedTask;
    });
```

### Implementing ancillary options

Option symbols can be defined in middleware that trigger ancillary functions not related to the implementations of the application's commands. Unlike symbols that are bound to model properties, ancillary symbols have a handling function and terminate the pipeline when matched. An example of this is the `--help` option. The handling function of a middleware option is a delegate that receives the `InvocationContext` and the current `ICommand` instance matched by the arguments (or the root command) and returns a task with the application exit code. The following example prints the sub commands for the current command in response to the user specifying the `--commands` option.

```csharp
builder.ConfigureMiddleware(middleware =>
    middleware.AddAncillaryOption(
        ["--commands"],
        (context, command) =>
        {
            foreach(var subCommand in commands)
            {
                Console.WriteLine(subCommand.Name);
            }

            return Task.FromResult(0);
        }
    ))
```
