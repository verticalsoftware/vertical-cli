using Microsoft.CodeAnalysis;
// ReSharper disable InconsistentNaming

namespace Vertical.Cli.Analyzers;

internal static class Descriptors
{
    // ReSharper disable once InconsistentNaming
    internal static readonly DiagnosticDescriptor VCLI0001 = new(
        id: nameof(VCLI0001),
        title: "Model missing a single, public constructor",
        messageFormat: "Model missing a single, public constructor",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);
    
    internal static readonly DiagnosticDescriptor VCLI0002 = new(
        id: nameof(VCLI0002),
        title: "Incompatible arity; binding is a collection type or array",
        messageFormat: "Incompatible arity; binding is a collection type or array",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);
    
    internal static readonly DiagnosticDescriptor VCLI0003 = new(
        id: nameof(VCLI0003),
        title: "Incompatible arity; binding is not a collection type or array",
        messageFormat: "Incompatible arity; binding is not a collection type or array",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);
    
    internal static readonly DiagnosticDescriptor VCLI0004 = new(
        id: nameof(VCLI0004),
        title: "Invalid identifier for option (must start with - or --)",
        messageFormat: "Invalid identifier for option (must start with - or --)",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);
    
    internal static readonly DiagnosticDescriptor VCLI0005 = new(
        id: nameof(VCLI0005),
        title: "Identifier required",
        messageFormat: "Identifier required",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);
    
    internal static readonly DiagnosticDescriptor VCLI0007 = new(
        id: nameof(VCLI0007),
        title: "Invalid route path pattern (must be space separated words consisting of alpha-numeric characters or -)",
        messageFormat: "Invalid route path pattern (must be space separated words consisting of alpha-numeric characters or -)",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        customTags: ["CompilationEnd"]);
    
    internal static readonly DiagnosticDescriptor VCLI0008 = new(
        id: nameof(VCLI0008),
        title: "Empty route path pattern",
        messageFormat: "Empty route path pattern",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        customTags: ["CompilationEnd"]);
    
    internal static readonly DiagnosticDescriptor VCLI0009 = new(
        id: nameof(VCLI0009),
        title: "Route must begin with the application name",
        messageFormat: "Route must begin with the application name '{0}'",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        customTags: ["CompilationEnd"]);
    
    internal static readonly DiagnosticDescriptor VCLI0010 = new(
        id: nameof(VCLI0010),
        title: "Incomplete property mapping",
        messageFormat: "Incomplete property mapping of type {0}: missing [{1}]",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        customTags: ["CompilationEnd"]);
    
    internal static readonly DiagnosticDescriptor VCLI0011 = new(
        id: nameof(VCLI0011),
        title: "Derived properties mapped",
        messageFormat: "One or more derived properties mapped in type {0}: [{1}]",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        customTags: ["CompilationEnd"]);
}