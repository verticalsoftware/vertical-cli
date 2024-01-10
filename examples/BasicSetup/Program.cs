// See https://aka.ms/new-console-template for more information

using Vertical.Cli;

var options = new CliOptions();

var rootCommand = RootCommand.Create<int>("program", root =>
{
    root.AddArgument<FileInfo>("root", 
        scope: SymbolScope.Descendents
        //, validator: Validator.Configure<FileInfo>(x => x.FileExists())
        );

    root.AddDescription("Manage builds in .Net");
    
    root.AddHelpOption(scope: SymbolScope.SelfAndDescendents);

    root.ConfigureSubCommand<Parameters>("push", cmd =>
    {
        cmd.AddSwitch("--no-compile", description: "Skip compilation")
            .AddSwitch("--no-symbols", description: "Don't build a program debug database.")
            .AddOption<string?>("--api-key", new[] { "-k" }, description: "Api key for the source server.")
            .AddOption<string?>("--source", new[] { "-s" }, description: "URI of the source server.")
            .AddOption<TimeSpan?>("--timeout", new[]{"-t"}, defaultProvider: () => TimeSpan.FromSeconds(30),
                description: "Max time to wait for connections.");

        cmd.AddDescription("Pushes a nuget package to a managed repository source.");
        
        cmd.SetHandler(parameters =>
        {
            //var json = JsonSerializer.Serialize(parameters, new JsonSerializerOptions{WriteIndented = true});
            //Console.WriteLine("parameters = " + json);
            return 0;
        });
    });

    root.ConfigureSubCommand<None>("shutdown-server", cmd =>
    {
        cmd.AddDescription("Shuts down the running builder server if it is running. The command executes the proper " +
                           "SIGTERM to enable shutdown. Health checks are stopped, and a shutdown message is sent " +
                           "to the cluster administrator prior to exit. The shutdown status code is stored with the " +
                           "cluster telemetry.");
    });
});

var dbgArgs = "biscuits|gravy"
    .Split('|'); 

rootCommand.Invoke(dbgArgs);