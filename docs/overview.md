# Overview

## Concepts

vertical-cli is a small console application framework. It's a _framework_ because the main application delegates control to it, and the framework routes control to handling mechanisms defined by the application.

## Steps of the setup

Setting up an application involves four fundamental steps:

- Step 1: Define object types that the application's user arguments will be mapped to. These objects can be expressed as classes, records, or interfaces.
- Step 2: Configure the parser by specifying the positional arguments, options, and switches the application's commands support.
- Step 3: Define the root command of the application (primary function) and any sub-commands (secondary functions).
- Step 4: Invoke the framework by passing it the application's `args`.

## Models

The primary purpose of the framework is to convert string arguments into strongly typed objects. Therefore, each command in the application that performs a function is paired with a model that is defined by the application.

The model contains the properties that the parser will bind arguments values to. The handling function of the command receives a composed model object and performs the application logic.

One of the benefits of the framework is the ability to convert an array of string arguments to a strongly typed object.  Commands defined by the application are provided instances of model objects that represent the options specified by the user. A model is simply a type where string arguments are mapped.

### Implementations

Realistically, the source generator can bind to any type its generated code has access to. Models can be declared as:

|Type|Remarks|
|---|---|
|Records|Arguments are mapped to the record's constructor parameters|
|Classes|Arguments are mapped to properties with a `set` or `init` accessor. Classes either conform to the `new()` type constraint or a factory activation function can be provided.
|Interfaces|The source generator creates a private implementation with `init` accessor properties.

### Property binding

Part of the model composition process is binding properties with converted argument values. Out of the box, the source generator has access to pre-defined value converters for the following types:
- Any type that implements `IParsable<T>`, which covers all integral structs in the `System` namespace
- Any `Nullable<T>` where the underlying value type is `IParsable<T>`
- `string`
- `System.IO` types `FileSystemInfo`, `DirectoryInfo` and `FileInfo`
- `System.Url` and `System.Version`
- Enums and `Nullable<TEnum>` (input casing is ignored)
- The following collection types provided the underlying type is any one of the above:
    - Arrays
    - `List<T>`, `Stack<T>`, `Queue<T>`, `HashSet<T>`, and `SortedSet<T>`
    - Immutable versions of the above collection types
    - Collection interfaces implemented by any of the above collection types. The source generator will use a compatible concrete type during instantiation.
        - Arrays are created for `IEnumerable<T>`, `IReadOnlyList<T>` and `IReadOnlyCollection<T>` 
        - `List<T>` is created for `IList<T>` and `ICollection<T>`
        - `HashSet<T>` is created for `ISet<T>` and `IReadOnlySet<T>`

### Interface model design pattern

It may be suprising that interfaces can be used since they are abstract, but they represent a powerful design mechanism that offers a lot of flexibility. In a complex application where commands have a shared interest in certain option properties, the models can be designed effectively with composition. The source generator will create its own implementations, while the application only needs to interact with the interface.

The following example shows how options common throughout the application can be defined separately and then composed for a command. The benefit of this pattern is each interface type can be configured once and shared throughout to all commands.

```csharp
interface IOutputOptions
{
    LogLevel LogLevel { get; }
}

interface IClientCredentials
{
    string SubscriptionId { get; }
    Secret ApiKey { get; }
}

interface IStorageOptions
{
    Uri StorageResource { get; }
}

// Command model composed with other common options
interface IPutBlobOptions : 
    IOutputOptions, 
    IClientCredentials,
    IStorageOptions
{
    FileInfo SourcePath { get; }
    bool Compress { get; }
}
```

## Symbols

The parser needs to be aware of what options, switches, and positional arguments the application supports. These can be thought of as symbol definitions. In order to complete the configuration, associations have to be made between each symbol and the property in the model the values of the symbol get bound to.

Symbols are conceptual objects that describe the relationship between a command line argument and the model property it maps to. Symbols are classified by their usage pattern on the command line.

### Types

The three types of symbols are described as follows:

- An _option_ is recognized by an identifier and a parameter value. The parameter value can be part of the same token as the identifier provided the identifier is terminated with a ':' or '=' character. Alternatively, the parameter value can be a separate token that follows the identifier. All of these usages are equivalent: 
    - `--log debug`
    - `--log:debug`
    - `--log=debug`.
- A _switch_ is a specialized option that represents a boolean value. The presence of its identifier in the arguments infers a value of `true` to its associated model property.
- A _positional argument_ is a parameter without an identifier. The parser recgonizes positional arguments when it cannot match an option or switch identifier.

### Identifier alias conventions

Options and switches have one or more identifier aliases. Valid identifier conventions are:

- A hyphen followed by a character (known as a short posix option), e.g `-a`
- Two hyphens followed by a kebab-cased name (known as a long form gnu option), e.g. `--log-level`
- A forward slash followed by a character or name (common in Windows shells). This convention is ignored by default, but can be enabled in the configuration.

A posix group is a hyphen followed by a character set. Each character in the set represents a discrete posix option. The parser expands the group token into separate tokens for matching, e.g. `-abc` is expanded to `-a`, `-b`, and `-c`. In this example, the first two expanded tokens can only be matched to switches `-a` and `-b`, while the last token `-c` can be matched to an option or a switch. If the last token is an option, then it can have an attached parameter value or a trailing parameter value argument, e.g. `-abc:value` is expanded to `-a`, `-b`, and `-c:value`.

### Arity

Arity represents the range count of values a symbol requires and/or accepts. The arity of a symbol is dependent on its associated property type.

- Scalar value symbols match `ZeroOrOne` or `One` value for binding.
- Multi-value symbols match `ZeroOrMore` or `OneOrMore` values for binding. The binding property in this case is a collection type.

### Positional argument precedence

Because positional arguments cannot be identified by alias, the parser must first exhaust all the tokens that can be matched to the other symbol types. The application sets the precedence of each positional argument symbol by using an ordinal sorting key.

> ⚠️ Important
>
> Pay extra attention to the arity of multi-valued positional arguments. A model composition should only have _one_ multi-value positional argument symbol defined, and it should have the highest precedence value so it is parsed last.

## Commands

Commands are discrete units of work in the application. An application must have a root command, but it can also have sub commands or a hierarchy. Commands can also be abstract in the sense that they don't perform a function, but provide a pathway to one or more sub commands. Consider a utility program called `archive` that compresses files. From the shell, the program can be invoked in one of two ways:

```bash
> archive create ./source.txt --out ./source.txt.gz
> archive extract .source.txt.gz --out ./source.txt
```

The root command of this application is `archive`. It is abstract because it does not perform a function, but it defines sub commands `create` and `extract`. The sub command will define a handling function that receives the composed model instance and a cancellation token. This function performs the application's function and returns an integer exit code.

### Invocation

When the application is fully configured, the last thing required is to pass the program's arguments to the framework. The framework will examine each argument, determine the command, compose a model instance with mapped binding values, and call the command's handling function.

## Up next

[Configuration](./configuration.md)