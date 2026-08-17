# Testing

### Overview

By default, when an application invokes `CommandLineApplication.RunAsync(args)`, an assertion framework inspects the configuration and will throw an exception if the configuration is invalid. Examples of configuration errors include:
- An application defined a switch called `--help` that conflicts with the system's help option
- A command is a dead end (has no handler implementation and doesn't define any sub commands)
- A type was detected that has no argument converter
- Variadic position arguments were misused

The configuration check is verbose and involves constructing symbol sets for each invokable command as opposed to at runtime, only a single symbol set is constructued based on the command the user selected.

### Unit testing the configuration

`CommandLineApplication` instances can be verified in a unit test by simply calling `AssertConfiguration()`. One of two approaches could be taken to give a unit test project access to the configuration:

```csharp
// Design 1: use static configuration
public static class MyApplication
{
    public static CommandLineApplication CreateInstance()
    {
        var rootCommand = new RootCommand("app");
        // configure
        
        var app = new CommandLineApplication(rootCommand);
        // configure
        
        return app;
    }
}

// Design 2: subclass CommandLineApplication
public sealed class MyCommandLineApplication : CommandLineApplication
{
    public MyApplication() : base(new RootCommand("app"))
    {
        // Configure        
    }
}
```

The following example illutsrates an xunit test:

```csharp
public class ApplicationConfigurationTest
{
    [Fact]
    public void AssertConfiguration_Does_Not_Throw()
    {
        var app = new MyCommandLineApplication();
        app.AssertConfiguration();
    }
    
}
```

### Disabling configuration checks during runtime

If your application asserts the configuration in a unit test, then repeating the checks at runtime becomes redundant. Disable configuration checks during runtime using a module initializer or setting the property shown below before calling `RunAsync()`.

```csharp
[ModuleInitializer]
static void Initialize()
{
    AssertionContext.Enabled = false;
}
```

### Abstracing the console

When the framework writes error messages or help files, it does so by using a console abstraction API. During normal operation, the abstraction simply wraps `System.Console`, but for unit testing, it may be helpful to capture output to a `StringBuidler` or `MemoryStream` and use tools such as [Verify](https://github.com/VerifyTests/Verify). Our own internal test suite does this. The following example shows how the console can be abstracted for a unit test:

```csharp
class TestConsole : IConsole
{
    private readonly StringWriter _writer = new StringWriter(new StringBuilder());
    
    public TextReader In => throw new NotSupportedException();
    
    public TextWriter Out => _writer;
    
    public bool IsOutputRedirected => true;
    
    public int DisplayWidth => 120;
    
    public override string ToString() => _writer.ToString();
}

// Unit test...
[Fact]
public async Task RunAsync_Writes_Expected_Result()
{
    // arrange
    var app = new CommandLineApplication(...);
    var console = new TestConsole();
    app.UseConsole(console);
    
    app.Configure();
    
    // act
    await app.RunAsync(args);
    
    // assert
    var output = console.ToString();
    return Verify(output);
}

```