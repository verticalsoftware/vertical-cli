// See https://aka.ms/new-console-template for more information

using System.IO.Compression;
using BasicSetup;
using Vertical.Cli;
using Vertical.Cli.Validation;

var rootCommand = RootCommand.Create<FileCopyParameters, Task<int>>(
    id: "copy",
    root =>
    {
        root.AddDescription("Copies a file, optionally compressing it.");

        root.AddArgument(
            id: "source",
            description: "Path to the source file",
            validator: Validator.Configure<FileInfo>(x => x.FileExists()));

        root.AddArgument(
            id: "dest",
            description: "Path to write the output file to",
            validator: Validator.Configure<FileInfo>(x => x.FilePathExists()));

        root.AddOption(
            id: "--compression",
            aliases: new[] { "-c" },
            description: "The compression type to use",
            defaultProvider: () => Compression.None);

        root.AddSwitch(
            id: "--overwrite",
            aliases: new[] { "-o" },
            description: "Whether to overwrite existing files");

        root.AddHelpOption();

        root.SetHandler(async (model, cancellationToken) =>
        {
            if (model.Dest.Exists && !model.Overwrite)
            {
                Console.WriteLine("Output file exists and will not be overwritten.");
                return -1;
            }
            
            await using var sourceStream = File.OpenRead(model.Source.FullName); 
            await using Stream destStream = model.Compression == Compression.GZip
                ? new GZipStream(File.OpenWrite(model.Dest.FullName), CompressionMode.Compress)
                : File.OpenWrite(model.Dest.FullName);
            
            await sourceStream.CopyToAsync(destStream, cancellationToken);
            return 0;
        });
    });

return await rootCommand.InvokeAsync(args);