// See https://aka.ms/new-console-template for more information

using System.Text.Json;
using Vertical.Cli;
using Vertical.Cli.Configuration;

var rootCommand = RootCommand.Create<Task<int>>(root =>
{
    root.AddArgument<string>("root", scope: SymbolScope.Descendents);

    root.ConfigureSubCommand<Parameters>("push", cmd =>
    {
        cmd.AddSwitch("--no-compile")
            .AddSwitch("--no-symbols")
            .AddOption<string>("--api-key", new[] { "-k" })
            .AddOption<string>("--source", new[] { "-s" })
            .AddOption("--timeout", new[]{"-t"}, defaultProvider: () => TimeSpan.FromSeconds(30));
        
        cmd.SetHandler(async parameters =>
        {
            await Task.CompletedTask;
            var json = JsonSerializer.Serialize(parameters, new JsonSerializerOptions{WriteIndented = true});
            Console.WriteLine("parameters = " + json);
            return 0;
        });
    });
});

rootCommand.ThrowIfInvalid();

try
{
    await rootCommand.InvokeAsync(args);
}
catch (CliArgumentException exception)
{
    Console.WriteLine(exception.Message);
}

Console.WriteLine("Hello");