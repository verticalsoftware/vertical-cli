using Vertical.Cli.Help;

namespace Vertical.Cli.ScenarioTests.Common;

public static class HelpResources
{
    public static readonly CommandHelpTopic Root = "Creates and extracts compressed, encrypted archive files.";

    public static readonly CommandHelpTopic CreateCommand = "Creates an archive set from one or more input files.";


    public static readonly SymbolHelpTopic CompressionTypeOption = new(
        "Compression type to use, one of gzip or brotli.",
        parameterSyntax: "type");


    public static readonly SymbolHelpTopic EncryptionTypeOption = new(
        "Encryption type to use to protect the files. If AES is used, the <KEY> references a symmetric key. If RSA is used, <KEY> references the recipient's public key.",
        parameterSyntax: "aes|rsa");

    public static readonly SymbolHelpTopic TimeoutOption = new("The maximum timespan for the operation to run, specific in hh:mm:ss.");

    public static readonly SymbolHelpTopic SecretOption = new(
        "The symmetric key or the recipient's public key used to encrypt the archive.",
        parameterSyntax: "key");

    public static readonly SymbolHelpTopic InputFilesArgument = new(
        "One or more input files to include in the archive.",
        parameterSyntax: "path");

    public static readonly SymbolHelpTopic CompressOutputFileOption = new(
        "Path and name of the compressed output file. For archives that are split into multiple files, the given path will be used as the name of the first file. Subsequent files will have a sequential numbering scheme applied to their names.");

    public static readonly SymbolHelpTopic ComputeShaSwitch = "Compute and display the SHA-1 hashes for each output file.";


    public static readonly SymbolHelpTopic OverwriteSwitch = "Overwrite any existing output files with the same name.";


    public static readonly SymbolHelpTopic IncludeMetadataSwitch = "Include source file metadata in the archive.";

    public static readonly SymbolHelpTopic OutputFileSplitSizeOption = new(
        "The maximum allowable size of each split output file.",
        parameterSyntax: "size[bkmg]");

    public static readonly SymbolHelpTopic PropertiesOption = new(
        "An addition metadata  key/value pair to include in the archive.",
        parameterSyntax: "key=value");

    public static readonly SymbolHelpTopic InputFileArgument = new(
        "Path to the first input file created with the compress command.",
        parameterSyntax: "path");

    public static readonly SymbolHelpTopic ExtractOutputPathOption = new(
        "Directory into which the extracted file(s) should be written (defaults to the current working directory).",
        parameterSyntax: "path");

    public static readonly CommandHelpTopic ExtractCommand = "Extracts an archive file or set.";

    public static readonly SymbolHelpTopic LogLevelDirective = new(
        "Configures verbosity of output logging. <level> = trace|debug|information|warning",
        parameterSyntax: "level");
}