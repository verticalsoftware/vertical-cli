using Vertical.Cli.Configuration;
using Vertical.Cli.Parsing;
using Vertical.Cli.Routing;

namespace Vertical.Cli.Binding;

internal delegate BindingContext BindingFactory(CliApplication application, 
    LinkedList<ArgumentSyntax> arguments,
    RouteDefinition route);