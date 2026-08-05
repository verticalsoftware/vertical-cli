# Directives

## Overview

In addition to full routing and model binding capability, the framework also provides various hooks into the control flow of the application in the form of directives and middleware.

## Directives

Directives are special symbols the user can provide that can have ancillary effects on the application. Directives are not tied to specific commands nor are they associated with the options model infrastructure. Rather, they are independent hooks that are controlled by the user and implemented by the application.

Directives are added by manipulating the `CommandLineApplication` object. Use the following methods to register a new directive:
- `AddDirective`
- `AddParameterizedDirective<TValue>`

When one or more directives are added, a built-in middleware component scans the user's input tokens and matches their identifiers. In the case of parameterized directives, it also enforces an optional or required parameter arity and performs value conversion.

The following example demonstrates how a directive can be configured that enables a user to configure the logging output level while leveraging the application options feature.

```csharp
var app = new CommandLineApplication(rootCommand);

// Other configuration

app.HandleParameterizedDirective(
    "log-level",
    // The attribute will ensure the string argument can be converted
    // to the LogLevel enum
    ([GeneratedConversion] info) => 
    {
        info.ApplicationOptions.Configure<AppOptions>(options => 
            options.LogLevel = info.Value);
            
        return Task.CompletedTask;            
    }
)

app.UseServices(context => 
{
    var logLevel = context.ApplicationOptions.GetOptions<AppOptions>().LogLevel;

    return new ServiceCollection()
        .AddLogging(builder => builder.SetMinimumLevel(loglevel))
        .BuildServiceProvider();
});


class AppOptions
{
    public LogLevel { get; set; } = LogLevel.Information;
}
```

> 💡Note
>
> The source generator add argument conversions automatically for all supported types in option models. However, it has no visibility to directive parameter types. Application's can either add an argument converter or decorate the `DirectiveEventInfo<TValue>` parameter with the `GeneratedConversion` attribute.