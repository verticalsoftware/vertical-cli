using CliDemo;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using Vertical.Cli;
using Vertical.Cli.Configuration;
using Vertical.Cli.Help;
using Vertical.Cli.Validation;

var app = new CliApplicationBuilder("sqz")
    // Register commands
    .Route("sqz")
    .RouteAsync<OperationOptions>("sqz create", async (options, cancel) => await App.CreateAsync(options, cancel))
    .RouteAsync<OperationOptions>("sqz extract", async (options, cancel) => await App.ExtractAsync(options, cancel))
    
    // Add help support
    .MapHelpSwitch(() => HelpProviders.CreateCompactFormatProvider(MyHelpFormatterOptions.Instance))
    
    // Map the option models
    .MapModel<CommonOptions>(map => map
        .Option(x => x.LogLevel, ["--log-level"], defaultProvider: () => LogLevel.Information)
        .Option(x => x.Cipher, ["--cipher"], validation: rule => rule.Must(
            evaluator: static (opt, value) => opt.EncryptionAlg == EncryptionAlg.None || value != null,
            message: "Cipher must be provided when encrypting/decrypting"))
        .Option(x => x.CompressionType, ["--compression"], defaultProvider: () => CompressionType.GZip)
        .Option(x => x.EncryptionAlg, ["--encryption"])
        // Demonstrate binding arbitrary values that aren't provided by CLI arguments
        .ValueBinding(x => x.ApplicationData, () => new object()))
    
    .MapModel<OperationOptions>(map => map
        .Switch(x => x.PrintSha, ["--sha"])
        .Switch(x => x.Overwrite, ["--overwrite"])
        .Argument(x => x.Source, validation: rule => rule.Exists())
        .Argument(x => x.Output, validation: rule => rule.Must(
            evaluator: static (opt, file) => opt.Overwrite || !file.Exists,
            message: "Output file exists (use --overwrite)")))
    
    // Add a custom converter
    .AddConverters([new LogLevelConverter()])
    
    // Build the application
    .Build();

try
{
    await app.InvokeAsync(args);
}
catch (CliArgumentException exception) when (exception is
                                             {
                                                 Error: CliArgumentError.PathNotCallable
                                                 or CliArgumentError.PathNotFound
                                             })
{
    AnsiConsole.MarkupLineInterpolated($"[red3_1]{exception.Message}[/]");
    AnsiConsole.WriteLine();

    if (exception.Route?.Path is { } failedPath)
    {
        await app.InvokeHelpAsync(failedPath);
    }
}
catch (CliArgumentException exception)
{
    AnsiConsole.MarkupInterpolated($"[red3_1]{exception.Message}[/]");
    AnsiConsole.WriteLine();
}
catch (Exception exception)
{
    AnsiConsole.WriteException(exception);
}
