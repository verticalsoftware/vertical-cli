# Customizing flow with middleware

### Overview

The framework uses middleware as an extensible pre-processing pipeline using _chain of responsibility_ patterened components. The components implement the following method signature:

```csharp
async Task InvokeAsync(InvocationContext context, Func<InvocationContext, Task> next);
```

The `InvocationContext` class contains data about the runtime environment such as a mutable token list, a reference to the console abstraction, configuration, etc. Middleware can:
- Inspect and mutate the input token list
- Set the exit code and short-circuit the pipeline
- Invoke the next component first, then react to the results
- Inject errors
- Read the configuration
- Request cancellation

### The default pipeline

The following middleware actions are configured by default (in order):

- For each token matched to a directive symbol, invoke the configured handler, then call the next middleware.
- For the first matched global switch, invoke the configured handler and return an exit code. When no switches are matched, call the next middleware.
- If the help option symbol is matched, display the contextual help article, otherwise call the next middleware.
- Call the next middleware; if one or more errors are found, display the help option suggestion.
- Call the next middleware; if one or more errors are found, print each error message.
- Parse and inject argument tokens found in files identified by annotation tokens, then call the next middleware.
- Add cancellation actions for `SIGTERM` and `SIGINT` signals, then call the next middleware. 

The final middleware builds the parse result and invokes the command handler.

### Customizing pre-processing with middleware

Middleware is defined by implementing a method with the signature shown above. It is registered into the pipeline using the `CommandLineApplication.ConfigureMiddleware` method. Middleware can be added as the first component, the last component, or in any other position by reconstructing the pipeline. The following example adds middleware to the start of the pipeline that listens for all application errors:

```csharp
app.ConfigureMiddleware(builder => builder
    .AddFirst(async (context, next) => 
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            context.OutputWriter.WriteLine(
                exception.ToString(),
                DisplayElement.Important);
        }
    }));
```

### Building a custom pipeline

If middleware needs to be placed anywhere besides the start or end of the default pipeline, the builder must be cleared and reconstructed with the new component in its desired place. The following illustrates building a new pipeline. Custom components can be added anywhere in the chain using the `AddLast` method.

```csharp
app.ConfigureMiddleware(builder => builder
    .Clear()
    .HandleDirectives()
    .HandleSwitches()
    .DisplayHelpArticles()
    .DisplayHelpOptionSuggestion()
    .DisplayInputErrors()
    .InjectResponseFileArguments()
    .HandleConsoleCancellation());
```

### Defining directives

Directives are symbols users can input that are handled outside of the command-model framework. They are used as hooks in the middleware pipeline the application can react to by manipulate some behavior or state. An example of this would be defining a directive that lets the user control the verbosity of output logging. While this would affect what messages are displayed when the application commands operate, it does not affect the functional behavior itself.

Directvies are defined in middleware and created with an identifier, an asynchronous handling method, and optionally a help topic object. Furthermore, a directive can be parameterized with a default provider and a validation action similar to options and position arguments. A user invokes the directive by enclosing the identifier in square brackets. The following example creates an application that allows the user to control logging. It introduces a parameterized directive.

### Defining middleware switches

Middleware switches are symbols that hijack the middleware pipeline when one is matched and performs a terminal action. An example of a middleware switch is one named `--version`, which prints version information about the application then exits. Like directives, middleware switches are accessible to the user globally. Middleware switches are defined using an identifier, an alias list, and an asynchronous handling method.

### Passing data through the framework

Data can be passed through the framework using an options system which is implemented as a simple value dictionary. Any delegates that receive an `InvocationContext` object have access to the options system. Using this object, data can be set and retrieved. Application data can be managed using the following APIs:

- `GetValueOrDefault<T>` - gets a value of type `T` or returns the provided default value. There can only be one value for each distinct type.
- `SetValue<T>` - sets the value of `T`. `T` is constrained to `notnull`.
- `Configure<TOptions>` - When provided, invokes a delegate the application uses to manipulate `TOptions`. After the delegate has concluded, the method returns the object. `TOptions` has the `class, new()` constraint, and the framework will create a singleton instance when the type is first referenced.

### Tieing it together

The following example demonstrates the options system, use of directives and switches, and adding a debugging middleware component. It can be found in the source repository's examples.

```csharp
var command = new RootCommand("app", "Says hello back to you.");
command.SetHandlerService<IOptions, Handler>();

var app = new CommandLineApplication(command);
app.ConfigureParser<IOptions>(parser => parser
    .ParseArgument(
        x => x.Name,
        ordinalPosition: 0,
        required: true,
        helpTopic: "Name of the current user."));

app.ConfigureMiddleware(middleware =>
{
    // Add a directive that lets the user control the log level
    middleware.AddDirective<LogLevel>(
        "log-level",
        ([GeneratedConversion] eventInfo) =>
        {
            eventInfo.Context.AppData.SetValue(eventInfo.Value);
            return Task.CompletedTask;
        },
        helpTopic: "Set the severity level of the logger.");
    
    // Add a version option
    middleware.AddSwitch(
        "Version",
        "--version",
        context =>
        {
            context.OutputWriter.WriteLine("Middleware demo v1.0");
            return Task.CompletedTask;
        },
        helpTopic: "Display the application's version.");
    
    // Add a middleware that lets the user attach the debugger
    middleware.AddLast(async (context, next) =>
    {
        context.OutputWriter.Write("Attach debugger then press any key...");
        _ = Console.ReadKey(intercept: true);
        context.OutputWriter.WriteLine();
        await next(context);
    });
});

app.ConfigureServices((context, services) =>
{
    services.AddSingleton<Handler>();
    
    var logLevel = context.AppData.GetValueOrDefault(LogLevel.Information);
    services.AddLogging(builder => builder
        .SetMinimumLevel(logLevel)
        .AddConsole());
});

app.Configure();
return await app.RunAsync(args);

// Model definition
[GeneratedBinding]
internal interface IOptions
{
    string Name { get; }
}

// Command handler
internal sealed class Handler(ILogger<Handler> logger) : IHandler<IOptions>
{
    /// <inheritdoc />
    public Task<int> HandleAsync(IOptions options, CancellationToken cancellationToken)
    {
        logger.LogDebug("Verbose logging enabled");
        logger.LogInformation("Hello {name}!", options.Name);
        return Task.FromResult(0);
    }
}
```

> ℹ️ Notes
> 
> The `[GeneratedConversion]` attribute signals the source generator to add an argument converter for the directive's parameter type. This is necessary since the type is not part of a command model.