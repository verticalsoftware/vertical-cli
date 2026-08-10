# Unit testing

## Abtracting the console

The console can be abstracted using the `IConsole` interface. This interface defines the input `TextReader`, output `TextWriter`, and a display width property. Application's can define their own console implementation that captures output and ensures content is preserved without ANSI formatting codes.

The following example demonstrates a simple capturing console:

```csharp
internal sealed class TestConsole : IConsole, IDisposable
{
    private readonly StringWriter _textWriter = new();
    
    public void Dispose() => _textWriter.Dispose();

    /// <inheritdoc />
    public TextReader In => throw new NotSupportedException();

    /// <inheritdoc />
    public TextWriter Out => _textWriter;

    /// <inheritdoc />
    public bool IsOutputRedirected => true;

    /// <inheritdoc />
    public int DisplayWidth => 120;

    public override string ToString() => _textWriter.ToString();
}
```

The console can be set in the application's setup:

```csharp
app.UseConsole(new TestConsole());
```

## Unit testing the configuration

When the `RunAsync` method is called, the framework performs a verbose inspection of the configuration and finds things that are missing or misconfigured. Assertion subjects include commands, symbol names, argument position, converters, etc. Unit test projects can call this method without staging arguments and calling `RunAsync`. Instead, it can call one of the following extension methods on the `CommandLineApplication` class:

|Name| Description|
|---|---|
|`AssertConfiguration`| Throws an exception is one or more issues are found. The exception contains detailed assertion information.|
|`GetConfigurationAssertions`| Returns a list of assertion objects, or an empty collection if the configuration is clean|