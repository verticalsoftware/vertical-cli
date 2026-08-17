# Help System

### Overview

By default, contextual help for the selected command will be displayed when the parser detects `--help` or `-?`. Application's can provide help topics for commands and all defined input symbols.

### Define help topics in code

The methods of `IParserBuilder<TModel>` let an application attach help content when symbols are defined. The type of content varies between subjects. Below is a summary of the types of help topics:

- `HelpTopic` - contain general remarks for switches
- `SymbolHelpTopic` - contain remarks and an optional custom parameter name for options, arguments, or directives. Custom parameter names are display in the option or argument list instead of the default names.
- `CommandHelpTopic` - contain general and extended sectional remarks

```csharp
// Attach help to a command
var command = new RootCommand(
    name: "compress",
    helpTopic: new CommandHelpTopic(
        remarks: "Compresses a file",
        extendedRemarks: 
        [
            new ExtendedRemarksSection(
                title: "Algorithms", 
                remarks: "Supports gzip, brotli, etc."),
            new ExtendedRemarksSection(
                title: "Encyrption", 
                remarks: "Supports AES or RSA."),
        ]));

// Attach help to an option
app.ConfigureParser<IOptions>(parser => parser
    .AddOption(
        x => x.Properties,
        aliases: "--prop",
        helpTopic: new SymbolHelpTopic(
            remarks: "A key/value pair property", 
            parameterName: "key=value")))
```

### Decoupling help content from code 

In larger applications, it may be more maintainable to define help content in a file. The help system has support for `xml` files, but applications can implement their own provider. Using an `xml` file requires configuring the help system as shown in the following example:

```csharp
var xmlHelpProvider = new XmlHelpProvider(() => File.OpenRead("help.xml"));
app.ConfigureHelp(options => options.HelpProvider = xmlHelpProvider);
```

The following illustrates the schema of an `xml` help resource file:

```xml
<?xml version="1.0" encoding="utf-8" ?>
<help>
    <topic type="command" id="<path>">
        <remarks>Remarks...</remarks>
        <sections>
            <section title="Section 1 title">
                section 1 remarks...
            </section>
            <section title="Section 2 title">
                section 2 remarks
            </section>
        </sections>
    </topic>
    
    <topic type="symbol" id="<fully-qualified-property>" parameter-name="<parameter-name>">
        Remarks... 
    </topic>
    
    <!-- middleware switches -->
    <topic type="symbol" id="(Switch).<Identifier>">
        Remarks...
    </topic>

    <!-- middleware directives -->    
    <topic type="symbol" id="(Directive).<identiier>>" parameter-name="parameter-name">
        Remarks...
    </topic>

    <!-- the system's help symbol -->
    <topic type="symbol" id="(Switch).Help">
        Remarks...
    </topic>
</help>
```

Attributes in the sample `xml`:

|Name|Description|
|---|---|
|`<path>`|The space separated path of the command, e.g. `dotnet nuget push`.|
|`<title>`|The title of a command's extended remarks section.|
|`<fully-qualified-property>`|A case-accurate property name, qualified with it's class type, e.g. `MyApplication.IConnectionOptions.UserName`.|
|`<parameter-name>`|A custom parameter name for options, arguments, and directive symbols.|
|`<identifier>`|The identifier used to create a directive or unbound option.|

### Implementing a custom help provider

Refer to the [IHelpProvider](https://github.com/verticalsoftware/vertical-cli/blob/main/src/lib/Vertical/Cli/Help/IHelpProvider.cs) and [DefaultHelpProvider](https://github.com/verticalsoftware/vertical-cli/blob/main/src/lib/Vertical/Cli/Help/DefaultHelpProvider.cs) for the API and the default implementation.
