# Help

The framework has a built-in help system that can be configured to show information about the application. It is not enabled by default and therefore must be activated by using the defaults of the middleware pipeline or explicitly adding the option.

### Enabling the help system

Help is enabled in middleware since it is an ancillary option using the `AddHelpOption` method.

```csharp
builder.ConfigureMiddleware(middleware =>
    middleware.AddHelpOption());)
```

