# Validating Input

## Overview

The framework comes with a purposeful, opt-in validation mechanism. When defining position argument, option, or switch symbols, value checks can be attached to each. If validation fails, the framework will report the apprpropriate error message (can be customized).

Each parser configuration method that is used to configure a symbol has a parameter called `Validate` that can be used to configure a value check or a series of value checks on the given input. There are several categories of extension methods that can be used to quickly configure basic validations. Alternatively, an application can implement its own validation logic using functions.

The delegate parameter is a context object (`ValidationEventInfo` class) that provides a reference to the complete model, the subject value, and a reference to the symbol being validated.

### Example

The following extends the initial example by checking if the input file to the compression program exists.

```csharp
app.ConfigureParser<ICompressOptions>(builder => builder
    .MapArgument(x => x.InputFile,
        ordinalPosition: 0,
        required: true,
    ➡️ validate: file => file.Exists())
    .MapArgument(x => x.OutputFile,
        ordinalPosition: 1,
        required: true)
    .MapOption(x => x.CompressionType,
        aliases: ["--compression"],
        defaultProvider: () => CompressionType.GZip)
    .MapSwitch(x => x.Overwrite)
);
```

The example can be extended to show how custom validation can be implemented. Here the code will check if the input file size is below a certain size.

```csharp
app.ConfigureParser<ICompressOptions>(builder => builder
    .MapArgument(x => x.InputFile,
        ordinalPosition: 0,
        required: true,
    ➡️ validate: context => 
        {
            context.MustExist();

            if (context.Value.Length < 1_000_000_000)
                return;

            context.Error("Input file cannot be larger than 1 GB.");
        })
    .MapArgument(x => x.OutputFile,
        ordinalPosition: 1,
        required: true)
    .MapOption(x => x.CompressionType,
        aliases: ["--compression"],
        defaultProvider: () => CompressionType.GZip)
    .MapSwitch(x => x.Overwrite)
);
```

### Applying rules to collection values

Collection values can be validated in the same manner. The validation delegate provides the same type of context object with the collection as the subject value, but has an additional method called `EachValue()`. This method accepts a delegate that can evaluate a context object scoped to each single value in the collection.

```csharp
builder.MapArgument(
    x => x.InputFiles,
    ordinalPosition: 0,
    validate: collection => collection
        .EachValue(element => elementContext.MustExist()),
    /* ... */);
```