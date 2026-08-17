# Testing

### Overview

By default, when an application invokes `CommandLineApplication.RunAsync(args)`, an assertion framework inspects the configuration and will throw an exception if the configuration is invalid. Examples of configuration errors include:
- An application defined a switch called `--help` that conflicts with the system's help option
- A command is a dead end (has no handler implementation and doesn't define any sub commands)
- A type was detected that has no argument converter
- Variadic position arguments were misused

The configuration check is verbose, and involves constructing symbol sets for each invokable command. Conversely at runtime, only a single symbol set is constructued based on the command the user selected.

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

If your application asserts the configuration in a unit test, then repeating the checks becomes redundant. Disable configuration checks during runtime using a module initializer.

```csharp
[ModuleInitializer]
static void Initialize()
{
    AssertionContext.Enabled = false;
}
```