// See https://aka.ms/new-console-template for more information

using NonInvocableCommand;
using Vertical.Cli;

var command = new RootCommand<Model>(
    "Command",
    "Description");

command.AddOption(x => x.Parameter, ["--parameter"], scope: CliScope.Descendants);
command.AddHelpSwitch();

try
{
    await command.InvokeAsync(args);
}
catch (CommandLineException ex) when (ex.Error == CommandLineError.NoCallSite)
{
    command.DisplayHelp(ex.Command);
}
