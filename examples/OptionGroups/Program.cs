﻿// See https://aka.ms/new-console-template for more information

using OptionsGroups;
using Vertical.Cli;
using Vertical.Cli.Help;

var command = new RootCommand<Model>("program", "Display option groups in help");

command
    .AddOption(x => x.LogLevel,
        ["--log-level"],
        description: "Verbosity of logger",
        optionGroup: "Common")
    .AddSwitch(x => x.Debug,
        ["--debug"],
        description: "Attaches a debugger",
        optionGroup: "Common")
    .AddOption(x => x.Host,
        ["--host"],
        description: "Url of the database host",
        optionGroup: "Database")
    .AddOption(x => x.Port,
        ["--port"],
        description: "Port using by the connector",
        optionGroup: "Database")
    .AddOption(x => x.Database,
        ["--database"],
        description: "Database to apply changes to",
        optionGroup: "Database")
    .AddOption(x => x.UserId,
        ["--user-id"],
        description: "Account used in the connection credential",
        optionGroup: "Database")
    .AddOption(x => x.Password,
        ["--password"],
        description: "Password used in the connection credential",
        optionGroup: "Database")
    .AddArgument(x => x.Command,
        description: "Command to execute")
    .AddOption(x => x.Properties,
        ["--property"],
        description: "Properties that define the command");

command.AddHelpSwitch();

command.ConfigureOptions(options =>
{
    options.HelpProvider = new DefaultHelpProvider(new DefaultHelpOptions
    {
        OptionGroups = 
        [
            "Database",
            "Common"
        ]
    });
});

command.Handle(options => { Console.WriteLine("Applied"); });

await command.InvokeAsync(["--help"]);