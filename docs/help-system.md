# The help system

## Overview

Guidance on the application's options can be displayed to the user using the built in help system. By default, if the user enters `--help or -?` after the application name (with or without a sub command), the list of arguments and options will be displayed contextually to the active command. Descriptions of the applications commands, options, and symbols can be configured to bring more meaning to the output.

### Adding help topics in code

Commands, options, arguments, switches, and directives can be configured with a `HelpTopic` instance when they are defined. The following data can be used with a help topic:

|Type|Help topic|
|---|---|
|Commands|General remarks plus zero or more extended remarks.
|Arguments and options|General remarks plus a customized parameter name. By default, the help system will  use the binding property name.|
|Directives|General remarks plus a customized parameter name (defaults to `value`).

The following example configures help for a command and an option.

```csharp
var command = new RootCommand(
    "fcopy",
    new CommandHelpTopic(
        "Copy a file.",
        [
            new CommandExtendedRemarks("Overview", "remarks..."),
            new CommandExtendedRemarks("Compression", "remarks...")
        ]
    ));

app.ConfigureParser<IOptions>(builder =>
    
    // Use a string for a help topic
    builder.ParseArgument(
        x => x.InputFile,
        ordinalPosition: 0,
        helpTopic: "Path to the input file.")

    // Customize a parameter name for an option
    builder.ParseOption(
        x => x.OutputFile,
        helpTopic: new SymbolHelpTopic(
            "Path to the output file",
            parameterName: "path"))
);                
```

### Adding help outside of code using XML

Applications may want to decouple help content from code. One way this can be accomplished is by using a help file structured as `xml`. A minimal example is shown below that illustrates the simple structure of the xml file.

```xml
<?xml version="1.0 encoding="utf-8"?>
<help>    
    
    <!-- Define help for a command. `id` is the qualified path name -->
    <topic type="command" id="app">
        <remarks>Provide the command's short description.</remarks>
        <sections>
            <section title="Section 1">
                Provide section remarks...
            </section
        </sections>
    </topic>

    <!-- Define help for an option, argument, or switch (parameter-name optional)-->
    <topic type="symbol" id="MyApplication.MyModelType.PropertyName" parameter-name="custom-name">
        Provide symbol remarks...
    </topic>

    <!-- Define help for a directive (parameter-name optional)-->
    <topic type="directive" id="name" parameter-name="custom-name">
        Provide directive remarks...
    </topic>
</help>
```

### Providing help using a different provider

The library defines the `IHelpProvider` interface which returns content strings that should be displayed by the help system. Application's can implement these to support custom resource formats. Review the existing implementation of `DefaultHelpProvider` as a starting point, where methods of that type can be overriden.

Inform the framework of a custom provider by configuring the help system options:

```csharp
app.ConfigureHelp(options => 
{
    // Set custom aliases for the help option:
    options.OptionAliases = ["--help", "-?"];

    // Customize the remarks for the help option:
    options.OptionRemarks = "Display help for this command.";

    // Set the xml provider (or the application's own type)
    options.HelpProvider = new XmlHelpProvider(() => File.OpenRead("help.xml"));
}
);
```

Lastly, an application can completely overtake the help system by sub-classing the `HelpArticleWriter` class and overriding the `WriteContent` method. 

### General content guidelines

Content guidelines:
- Command `id` attributes are the qualified path name for the command. For a root command, this is the application name defined in the `RootCommand` constructor. For sub commands, it is a path string composed of parent command names and the sub command name separated by spaces, e.g., `dotnet nuget push`.
- Option and switch id attributes are the fully qualified model interface type name and the property name.
- All id attributes are case-sensitive.
- Do not enclose custom parameter names with anchoring characters like `[]` or `<>`. The help system adds these automatically using the semantics of the symbol type.
- Do not justify the words in a remarks element. The formatter will wrap and align the content responsively to the output display width.