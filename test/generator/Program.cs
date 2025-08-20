// See https://aka.ms/new-console-template for more information

using GeneratorTestApp;
using Vertical.Cli;

var command = new RootCommand<OptionsClass>(
    "app",
    async (options, cancellation) =>
    {
        while (await options.InputStream.ReadLineAsync(cancellation) is { } line)
        {
            Console.WriteLine(line);
        }

        return 0;
    });

var builder = new CommandLineBuilder(command)
    .ConfigureModel<OptionsClass>(model => model
        .AddOption(x => x.Property)
        .BindStandardInput(x => x.InputStream));

return await builder.BuildAndRunAsync([]);        