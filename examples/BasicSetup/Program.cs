// See https://aka.ms/new-console-template for more information

using Vertical.Cli;
using Vertical.Cli.Validation;

var options = new CliOptions();
options.ModelBinders.Add(new ParametersBinder());
options.AddConverter(str =>
{
    var split = str.Split(',');
    return new Point(int.Parse(split[0]), int.Parse(split[1]));
});
options.AddValidator<TimeSpan>(
    x => x.LessThanOrEquals(TimeSpan.FromSeconds(60),
    message: () => "Timeout must be 60 seconds or less."));

var rootCommand = RootCommand.Create<Task>("program", root =>
{
    root.AddArgument<FileInfo>("root", 
        scope: SymbolScope.Descendents
        //, validator: Validator.Configure<FileInfo>(x => x.FileExists())
        );

    root.AddDescription("Manage builds in .Net");

    root.AddHelpOption();

    root.ConfigureSubCommand<Parameters>(
        id: "push",
        cmd =>
        {
            cmd.Family("--no-compile", description: "Skip compilation")
                .Family("--no-symbols", description: "Don't build a program debug database.")
                .AddOption<string?>("--api-key", new[] { "-k" }, description: "Api key for the source server.")
                .AddOption<string?>("--source", new[] { "-s" }, description: "URI of the source server.")
                .AddOption<TimeSpan?>("--timeout", new[] { "-t" }, defaultProvider: () => TimeSpan.FromSeconds(30),
                    description: "Max time to wait for connections.");

            cmd.AddDescription("Pushes a nuget package to a managed repository source.");

            cmd.SetHandler(parameters =>
            {
                //var json = JsonSerializer.Serialize(parameters, new JsonSerializerOptions{WriteIndented = true});
                //Console.WriteLine("parameters = " + json);
                return Task.CompletedTask;
            });
        });

    root.ConfigureSubCommand<None>("shutdown-server", cmd =>
    {
        cmd.AddDescription("Shuts down the running builder server if it is running. The command executes the proper " +
                           "SIGTERM to enable shutdown. Health checks are stopped, and a shutdown message is sent " +
                           "to the cluster administrator prior to exit. The shutdown status code is stored with the " +
                           "cluster telemetry.");
    });
}, options);

var dbgArgs = "push|/var/lib/myproject.csproj|--no-compile|-k=*secret"
    .Split('|'); 

//var context = CallSiteContext.Create(rootCommand, dbgArgs, 0);




//Console.WriteLine(context.CallSite.State);