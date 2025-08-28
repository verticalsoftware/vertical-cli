# vertical-cli

Fully featured command line argument parsing library.

## Features

- Built for complex CLI applications
- Converts string arguments to types
- Supports command hierarchies
- Binds arguments to any `IParsable<T>` and their `Nullable<T>` compliments, enums, strings, collections, and more
- Leverages a source generator for a reflection free, AOT-trimmable application
- Integrates a fully customizable help system
- Supports response files and directives
- Extensible with other add-ons

## Installation

Add the package to your application

```bash
> dotnet add package vertical-cli --prerelease
```

## Documentation

- [Conceptual overview](./docs/overview.md)
- [Configuration](./docs/configuration.md)
- [Advanced concepts](./docs/advanced.md)
- [Middleware](./docs/middleware.md)
- [Adding help](./docs/help.md)
