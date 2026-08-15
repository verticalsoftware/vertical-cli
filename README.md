# vertical-cli

Develop CLI applications using a rich argument binding framework.

![NuGet Version](https://img.shields.io/nuget/vpre/vertical-cli?label=vertical-cli)
![NuGet Version](https://img.shields.io/nuget/vpre/vertical-cli-dependencyinjection?label=vertical-cli-dependencyinjection)

## Features

- Parses position arguments and  GNU short and long format options and switches.
- Binds parsed argument values to strongly typed models using reflection-free source generation.
- Supports command hierarchy.
- Injects arguments using response file annotations.
- Provides a customizable help system.
- Defines a rich user input validation API.
- Supports dependency injection using the [vertical-cli-dependencyinjection](https://www.nuget.org/packages/vertical-cli-dependencyinjection) companion package.

## Installation

```shell
$ dotnet pcakage add vertical-cli --prerelease
```

## Quick start

```csharp
// Define a command
var rootCommand = new RootCommand("app");

// Provide application logic
rootCommand.SetHandler<IOptions>(async (options, cancellationToken) => 
{
    // TODO: perform application work and return an exit code
    await LoginAsync(options.UserName, options.Password);
    return 0;
});

// Create the application
var app = new CommandLineApplication(rootCommand);

// Configure the parser
app.ConfigureParser<IOptions>(parser => parser
    .AddOption(x => x.UserName)
    .AddOption(x => x.Password));

// Applies generated configuration
app.Configure();

// Run the application
return await app.RunAsync(args);

// Define the model for the command
[GeneratedBinding]
public interface IOptions
{
    string UserName { get; }
    string Password { get; }
}
```

