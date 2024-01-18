using PublicApiGenerator;

namespace Vertical.Cli;

[UsesVerify]
public class ApiShippingTests
{
    [Fact]
    public Task PublicApiIsApproved()
    {
        var assembly = typeof(RootCommand).Assembly;
        var publicApi = assembly
            .GeneratePublicApi(new()
            {
                IncludeAssemblyAttributes = false,
                ExcludeAttributes = new string[]
                {
                    "System.Runtime.CompilerServices.CompilerGenerated",
                    "System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage"
                }
            });

        return Verify(publicApi);
    }
}