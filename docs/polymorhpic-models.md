# Command models

### Overview

Simple applications will likely only define a root command, and along with it, a single command model. More complex applications define hierarchies of commands that may share options or arguments. Command models are defined using interfaces, and this design was purposefully chosen so that shared input components can be individually defined and then command models can be _composed_.

Consider an application that acts as a suite of tools. Each tool requires authentication to a service. Additionally, the user can control logging output. Polymorphic command models provide separation of [configuration] concerns, and helps elimintate the repeitition of configuring shared symbols.

```csharp
// Models
interface IAuthenticationOptions
{
    string UserId { get; }
    string Password { get; }
    bool UseBrowserFlow { get; }
}

interface ILoggingOptions
{
    LogLevel LogSeverityLevel { get; }
    FileInfo? LogOutputFile { get; }
}

// Command model using composition
[GeneratedBinding]
interface IUploadOptions : IAuthenticationOptions, ILoggingOptions
{
    FileInfo[] FilePaths { get; }
}

// Configuration
app.ConfigureParser<IAuthenticationOptions>(parser => /* configure */);
app.ConfigureParser<ILoggingOptions>(parser => /* configure */);
app.CofigureParser<IUploadOptions>(parser => /* configure */);
```

### The source generator

The `[GeneratedBinding]` attribute is a source generator signal. It will produce the folliwing code for each decorated inteface type:
- A `file` scoped class is generated that implements the interface.
- A method is defined that constructs model instances using the parser's result.
- Converter methods are registered to handle the types of property assignments.

> ℹ️ Note
> 
> The `GeneratedBinding` attribute only needs to be applied to final composed command model types, not every super type. 

This attribute can be applied to an interface or a parameter:

```csharp
command.SetHandler<IUploadOptions>(async 
    ([GeneratedBinding] IUploadOptions options,
    CancellationToken cancellationToken) => 
    {        
        // ...
    });
```

### Invoking the source generator

The source generator creates a single file that contains extension methods to the `CommandLineApplication` class. The `Configure()` method enables the functionality of the generated code, and `ConfigureAndRunAsync(args)` method combines the `Configure` and `RunAsync` methods. `Configure()` can be called at any time.

### Binding private values

Command models can have properties that represent values private to the application. In this case, the values aren't provided by user input. Rather, the application maps these value inputs itself. An example use case of this is when the application wants to provide commands with some type of state object. This capability is provided in the `ModelBuilder.MapBindingInfoValue` API. The `PropertyBindingInfo` class provides access to the parse result and application options.

This example demonstates how applications can bind values to a model instance without user input.

```csharp
// Add the property to a model
interface IOptions
{
    object ApplicationData { get; }
    TextReader Input { get; }
}

// Configuration examples

// Map a static value
app.ConfigureModel<IOptions>(model => model.MapStaticValue(
    x => x.ApplicationData,
    value: myAppData));

// Map the input stream
app.ConfigureModel<IOptions>(model => model.MapInputStream(
    x => x.Input));
```

### Manually binding a model

The source generate handles this, but if an application's needs extend beyond what is provided, it can manually provider a model binding. This is a method that receives a `BindingContext<TModel>` and is expected to return the model instance that is provided to the command.

The following illustrates what the source generator would compose for a simple model. Note that when this method is called, the following actions have been completed:
- User input arguments were parsed and paired with their respective bindings.
- String arguments have been converted to their expected property types.
- Arity and other input checks have passed.
- Application defined validations were successfully resolved.

```csharp
// Record is used here since this example is bypassing the source generator.
public record ConnectionOptions(
    string Server,
    string Database,
    int Port,
    string UserId,
    string Password);

// Configuration
app.ConfigureModel<ConnectionOptions>(model => model.SetBinder(
    bindingContext => new ConnectionOptions(
        bindingContext.GetValue(x => x.Server),
        bindingContext.GetValue(x => x.Database),
        bindingContext.GetValue(x => x.Port),
        bindingContext.GetValue(x => x.UserId),
        bindingContext.GetValue(x => x.Password)));
```