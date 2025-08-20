// See https://aka.ms/new-console-template for more information

using System.Text.RegularExpressions;
using Vertical.Archiver;
using Vertical.Cli;
using Vertical.Cli.Configuration;
using Vertical.Cli.Conversion;
using Vertical.Cli.Help;
using Vertical.Cli.Validation;

// for (var c = 0; c < 256; c++)
// {
//     Console.WriteLine($"\x1b[38;5;{30 + c}mColor {c}\x1b[0m");
// }


var rootCommand = new RootCommand(
    "archive",
    helpTag: "create and manage archive files using GZip or Deflate compression, optionally securing " +
             "artifacts using AES (symmetric) or RSA (asymmetric) encryption.");

rootCommand.AddCommand(
    new Command<CreateOptions>(
        "create",
        CreateOperation.Handle,
        "Creates a new archive file"));

rootCommand.AddCommand(
    new Command<ExtractOptions>(
        "extract",
        ExtractOperation.Handle,
        "Extracts an existing archive file"));

var app = new CommandLineBuilder(rootCommand)
    .ConfigureModel<IArchivingOptions>(model => model
        .AddOption(x => x.Compression,
            aliases: ["-c", "--compression"],
            helpTag: "Compression type to use [GZip | Deflate]")
        .AddOption(x => x.Encryption,
            aliases: ["-e", "--encryption"],
            helpTag: "Encryption algorithm to use [AES | RSA]")
        .AddOption(x => x.Timeout,
            helpTag:
            "A time interval that specifies how long the operation is permitted to run (format as <value>[h|m|s])\nExamples:\n- 1h\n= 20m",
            configureValidation: args => args.LessThan(TimeSpan.FromSeconds(30)))
        .AddOption(x => x.Checksum,
            helpTag: "The hashing algorithm to use when printing the checksum [Md5 | Sha1 | Sha256]")
        .AddOption(x => x.KeyFile,
            helpTag:
            "Path to the symmetric key file (required when [-e, --encryption]=RSA). Create operations use a public key file while extract operations use the complimentary private key file")
        .AddOption(x => x.Passphrase,
            helpTag: "Passphrase used to encrypt/decrypt the archive file (required when [-e, --encryption]=AES",
            configureValidation: args => args.MinimumLengthOrNull(10)))
    .ConfigureModel<ICreateOptions>(model => model
        .AddCollectionArgument(x => x.Sources,
            precedence: 0,
            name: "SOURCE",
            arity: CollectionArity.OneOrMore,
            setBindingOptions: opt => opt.SetDefaultValue([new DirectoryInfo(Directory.GetCurrentDirectory())]),
            helpTag: "One or more sources to include in the archive, each being either a file or directory")
        .AddOption(x => x.OutputPath,
            aliases: ["--out"],
            helpTag: "Name of the archive file to write",
            setBindingOptions: opt =>
                opt.SetDefaultValue(new FileInfo(Path.GetFullPath($"./archive_{DateTimeOffset.Now:yyyyMMdd}")))))
    .ConfigureModel<IExtractOptions>(model => model
        .AddOption(x => x.SourcePath,
            aliases: ["--in"],
            arity: BasicArity.One,
            helpTag: "Path to the archive file to extract")
        .AddOption(x => x.OutputPath,
            aliases: ["--out"],
            setBindingOptions: opt => opt.SetDefaultValue(new DirectoryInfo(Directory.GetCurrentDirectory())),
            helpTag: "Path where extracted file(s) will be written (defaults to current directory)"))
    .ConfigureModel<CreateOptions>(model => model.UseBinder(context => new CreateOptions(
        context.GetValue(x => x.Compression, Converters.Enum<CompressionType>()),
        context.GetValue(x => x.Encryption, Converters.Enum<EncryptionType>()),
        context.GetValue(x => x.Timeout, MyConverters.ToTimeSpan),
        context.GetValue(x => x.Checksum, Converters.Enum<Checksum>()),
        context.GetValue(x => x.KeyFile, Converters.FileInfo),
        context.GetValue(x => x.Passphrase, Converters.Default),
        context.GetCollectionValue<FileSystemInfo, FileSystemInfo[]>(x => x.Sources, Converters.FileSystemInfo,
            values => [..values]),
        context.GetValue(x => x.OutputPath, Converters.FileInfo))))
    .ConfigureModel<ExtractOptions>(model => model.UseBinder(context => new ExtractOptions(
        context.GetValue(x => x.Compression, Converters.Enum<CompressionType>()),
        context.GetValue(x => x.Encryption, Converters.Enum<EncryptionType>()),
        context.GetValue(x => x.Timeout, MyConverters.ToTimeSpan),
        context.GetValue(x => x.Checksum, Converters.Enum<Checksum>()),
        context.GetValue(x => x.KeyFile, Converters.FileInfo),
        context.GetValue(x => x.Passphrase, Converters.Default),
        context.GetValue(x => x.SourcePath, Converters.FileInfo),
        context.GetValue(x => x.OutputPath, Converters.DirectoryInfo))))
    .Build();

string[] createArgs =
[
    "create",
    "--timeout:15s",
    "--out:/usr/Documents",
    "-e:rsa",
    "--key-file:./usr/ssh/id30519",
    "--passphrase:secret"
];

//return await app.RunAsync(["-?"]); 
return await app.RunAsync(["create", "-?"]); 
//return await app.RunAsync(createArgs);
//return await app.RunAsync(args);