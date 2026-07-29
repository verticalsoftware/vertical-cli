using Vertical.Cli.Help;

namespace BasicDemo;

public static class Help
{
    public static readonly CommandHelpTopic Root = new(
        remarks: "Provides various compression and encryption based file utilities.");
    
    public static readonly CommandHelpTopic Compress = new(
        remarks: "Compresses one or more source files into a proprietary archive file using either the gzip or brotli algorithms."
        );

    public static readonly SymbolHelpTopic SourceFiles = new(
        remarks: "One or more source files to add to the compression archive.",
        parameterSyntax: "path");

    public static readonly SymbolHelpTopic OutputFile = new(
        remarks: "Path used when creating the output file.",
        parameterSyntax: "path");

    public static readonly SymbolHelpTopic PrintSha = new(
        remarks: "Compute and display the sha-256 of the output file.");

    public static readonly SymbolHelpTopic? CompressType = new(
        remarks: "The compression algorithm to use, can be either gzip or brotli.",
        parameterSyntax: "gzip|brotli");

    public static readonly SymbolHelpTopic? LogDirective = new(
        remarks: "Verbosity of output logging (trace, debug, or info).",
        parameterSyntax: "severity");

    public static SymbolHelpTopic? SplitSize = new(
        remarks: "Split size, in kb, to optimize the output file.",
        parameterSyntax: "size-kb");

    public static SymbolHelpTopic? Timeout = new(
        remarks: "Maximum time to allow for the operation.",
        parameterSyntax: "mm:ss");
}