using PublicApiGenerator;

namespace Vertical.Cli.DependencyInjection.UnitTests;

public class ShippedApiTest
{
    [Fact]
    public Task Releasing_Approved_Api() => Verify(typeof(CommandLineApplication)
        .Assembly
        .GeneratePublicApi());
}