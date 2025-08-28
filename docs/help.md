# Help

The framework has a built-in help system that can be configured to show information about the application. It is not enabled by default and therefore must be activated by using the defaults of the middleware pipeline or explicitly adding the option.

### Enabling the help system

Help is enabled in middleware since it is an ancillary option using the `AddHelpOption` method. 

```csharp
builder.ConfigureMiddleware(middleware =>
    middleware.AddHelpOption());
```

### Adding help content

Descriptions and other help content can be associated with commands, directives, and symbols when they are created using the `CommandHelpTag`, `DirectiveHelpTag`, and `SymbolHelpTag` types, respectively. Note the first two types will implicitly convert a string description to the expected type.

### Customizing the help option

The default identifiers for the help option are `--help` and `-?`. This can be customized using the `CommandLineOptions` API.

```csharp
builder.ConfigureOptions(options => options
    .HelpOptionAliases = ["--help"]);
```

### Using a custom help provider

The help system constists of the following components:

|Component|Description|
|---|---|
|`IHelpTextWriter`|Renders help elements to the console abstraction. The framework provides the `NonDecoratedHelpWriter` and the `ColoredOutputHelpWriter` implementations.|
|`ILayoutEngine`|Formats, aligns, trims, and wraps help content according to a particular layout. The framework provides the `CompactLayoutEngine` and the `UnixStyleLayoutEngine` implementations.|
|`IHelpResourceManager`|Provides the content to display for symbols, directives, and commands. The default implementation uses the help content defined in code.|
|`IHelpProvider`|Coordinates the above components to render contextualized help content for the selected command|

The client application can sublcass or provide its own implementations of these interfaces and then provide a `IHelpProvider` factory function in the `CommandLineOptions` API. The factory function provides the `IConsole` instance and is expected to return an `IHelpProvider` object.

```csharp
builder.ConfigureOptions(options => 
{
    options.HelpProviderFactory = console =>
    {
        var helpTextWriter = new ColoredOutputHelpWriter(console.Out);
        var layoutEngine = new CompactLayoutEngine(
            helpTextWriter, 
            console.DisplayWidth);
        var resourceManager = new HelpTagResourceManager();

        return new DefaultHelpProvider(layoutEngine, resourceManager);
    };
});
```