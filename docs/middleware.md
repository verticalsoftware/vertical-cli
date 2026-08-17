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
- For the fist matched global switch, invoke the configured handler and return an exit code. When no switches are matched, call the next middleware.
- If the help option symbol is matched, display the contextual help article, otherwise call the next middleware.
- Call the next middleware; if one or more errors are found, display the help option suggestion.
- Call the next middleware; if one or more errors are found, print each error message.
- Parse and inject argument tokens found in files identified by annotation tokens, then call the next middleware.
- Add cancellation actions for `SIGTERM` and `SIGINT` signals, then call the next middleware. 