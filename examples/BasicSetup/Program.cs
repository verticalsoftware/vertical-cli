using System.Text.Json;
using BasicSetup;
using Vertical.Cli;
using Vertical.Cli.Configuration;
using Vertical.Cli.Validation;

var root = new RootCommand<Model, Task>("cp", "Copies files");
root
    .AddOption(x => x.LogLevel, ["--log-level"],
        description:
        "The verbosity level of log output. Can be one of: Trace, Debug, Information, Warning, Error, or Critical.")
    .AddArgument(x => x.Sources,
        description:
        "The name of the source file. This could be an absolute or relative path, or a directory/globbing pattern.",
        validation: v => v.Each<string>(c => c.HasMinLength(10)))
    .AddOption(x => x.DirectoryInfo, ["--out"], validation: v => v.ExistsOrIsNull())
    .AddOption(x => x.FileInfo, ["--in"])
    .AddOption(x => x.BaseUrl, ["--endpoint"])
    .AddOption(x => x.Port, ["--port"], validation: v => v.GreaterThan((uint)3000))
    .AddOption(x => x.UserId, ["-u"])
    .AddHelpSwitch(Task.CompletedTask);

root.SetHandler(async (model, cancellationToken) =>
{
    await Task.CompletedTask;
    
    Console.WriteLine(JsonSerializer.Serialize(new
    {
        model.LogLevel,
        Value = string.Join(',', model.Sources),
        DirectoryInfo = model.DirectoryInfo?.FullName,
        FileInfo = model.FileInfo?.FullName,
        BaseUrl = model.BaseUrl?.ToString(),
        model.Port,
        model.UserId
    }, new JsonSerializerOptions
    {
        WriteIndented = true
    }));
});

var testArgs =
    "snips --log-level information --out . --in .\\Program.cs --endpoint https://google.com --port 3306"
        .Split(' ');

try
{
    await root.InvokeAsync(args);
}
catch (CommandLineException exception)
{
    Console.WriteLine(exception.Message);
}

Console.WriteLine("done");