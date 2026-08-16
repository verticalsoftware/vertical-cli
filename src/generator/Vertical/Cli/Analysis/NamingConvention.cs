namespace Vertical.Cli.Analysis;

public static class NamingConvention
{
    public const string VerticalCliNs = "Vertical.Cli";
    public const string VerticalCliMiddlewareNs = $"{VerticalCliNs}.Middleware";
    public const string CommandLineApplicationClassFqName = $"global::{VerticalCliNs}.CommandLineApplication";
    public const string VerticalCliBindingNs = $"{VerticalCliNs}.Binding";
    public const string VerticalCliConversionNs = $"{VerticalCliNs}.Conversion";
    public const string GeneratedBindingAttributeMetadataName = $"{VerticalCliBindingNs}.GeneratedBindingAttribute";
    public const string GeneratedConversionAttributeMetadataName = $"{VerticalCliConversionNs}.GeneratedConversionAttribute";
    public const string ConvertersCallFqName = $"global::{VerticalCliConversionNs}.Converters";
    public const string BindingContextClass = $"global::{VerticalCliBindingNs}.BindingContext<{{0}}>";
    public const string ParameterizedMiddlewareDirectiveInfoName = $"{VerticalCliMiddlewareNs}.ParameterizedMiddlewareDirectiveInfo`1";
}