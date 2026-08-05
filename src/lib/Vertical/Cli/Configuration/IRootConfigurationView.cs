using Vertical.Cli.Conversion;
using Vertical.Cli.Help;
using Vertical.Cli.Invocation;
using Vertical.Cli.IO;
using Vertical.Cli.Middleware;
using Vertical.Cli.Utilities;

namespace Vertical.Cli.Configuration;

public interface IRootConfigurationView : IConversionProvider
{
    RootCommand RootCommand { get; }

    ModelConfiguration GetModelConfiguration(Type modelType);

     MiddlewareDelegate GetMiddlewarePipeline();
    
    OptionsManager OptionsManager { get; }
    
    IConsole Console { get; }
    
    OutputFormatter OutputFormatter { get; }
    
    HelpSystemOptions HelpOptions { get; }
    
    bool HasClientServiceContext { get; }

    Stream GetAnnotationStream(string resource);

    IReadOnlyList<IDirectiveSymbol> GetDirectives();

    bool HasArgumentConverter(Type type);

    bool HasCollectionConverter(Type elementType, Type collectionType);
}