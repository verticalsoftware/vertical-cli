// using Vertical.Cli;
// using Vertical.Cli.Conversion;
//
// namespace BasicDemo;
//
// public static class Generated
// {
//     public static void Configure(this CommandLineApplication app)
//     {
//         app.AddArgumentConverter(Converters.FileInfo);
//         app.AddArgumentConverter(Converters.Parsable<bool>());
//         app.AddArgumentConverter(Converters.Parsable<int>());
//         app.AddArgumentConverter(Converters.Enum<CompressionAlgorithm>());
//         app.AddCollectionConverter(Converters.Array<FileInfo>());
//         
//         app.ConfigureModel<IOptions>(builder => builder.SetBinder(context => new Options(
//             CompressionType: context.GetValue(x => x.CompressionType),
//             SourceFiles: context.GetValue(x => x.SourceFiles),
//             OutputFile: context.GetValue(x => x.OutputFile),
//             PrintSha: context.GetValue(x => x.PrintSha),
//             SplitSizeKb: context.GetValue(x => x.SplitSizeKb),
//             Timeout: context.GetValue(x => x.Timeout))));
//     }
// }