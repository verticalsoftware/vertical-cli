# vertical-cli

Minimal, AOT friendly command line arguments parser.

## Quick start

### Install

```shell
dotnet add package vertical-cli --prerelease
```
### Configure and run

```csharp
// Define a model
public record ZipOptions(FileInfo Source, FileInfo Destination, bool Overwrite);

// Build an application with two commands
var app = new CliApplicationBuilder("gzip")
    .RouteAsync<ZipOptions>("gzip create", async (model, cancelToken) => {
        if (model.Destination.Exists && !model.Overwrite){
            Console.WriteLine("Target file already exists");
            return -1;
        }
        await using var inputStream = File.OpenRead(model.Source);
        await using var outputStream = File.OpenWrite(model.Destination);
        await using var zipStream = new GZipStream(outputStream, CompressionMode.Compress);
        await inputStream.CopyToAsync(zipStream, cancelToken);

        Console.WriteLine($"Compressed file {model.Destination} created.");
    })
    .RouteAsync<ZipOptions>("gzip extract", async (model, cancelToken) => {
        if (model.Destination.Exists && !model.Overwrite){
            Console.WriteLine("Target file already exists");
            return -1;
        }
        await using var inputStream = File.OpenRead(model.Source);
        await using var outputStream = File.OpenWrite(model.Destination);
        await using var zipStream = new GZipStream(inputStream, CompressionMode.Decompress);
        await zipStream.CopyToAsync(outputStream, cancelToken);

        Console.WriteLine($"File {model.Source} extracted.");
    })
    .MapModel<ZipOptions>(map => map
        .Argument(x => x.Source)
        .Argument(x => x.Destination)
        .Switch(x => x.Overwrite, ["--overwrite"]))
    .Build();

await app.InvokeAsync(args);

// Run:
// $ gzip picture.png picture.gz --overwrite
```

## Features

- Binds command line arguments to strongly typed models
- Configure positional arguments. options and switches using short and long form notations
- Define a hierarchy of commands each with derived models
- Uses a source generator to bind models removing the need for reflection (AOT friendly)
- Uses analyzers to provide wranings and errors for common misconfiguration issues
- Display generated help content

See full [docs](https://github.com/verticalsoftware/vertical-cli/blob/main/assets/docs.md).