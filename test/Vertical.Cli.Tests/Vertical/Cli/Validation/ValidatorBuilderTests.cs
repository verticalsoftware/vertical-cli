using NSubstitute;

namespace Vertical.Cli.Validation;

public class ValidatorBuilderTests
{
    [Fact]
    public void ThrowsWhenNoInstancesConfigured()
    {
        Assert.Throws<InvalidOperationException>(() => new ValidationBuilder<string>().Build());
    }

    [Fact]
    public void AddsInstanceToCollection()
    {
        var validator = Substitute.For<Validator<string>>();
        var compositeValidator = Validator.Configure<string>(x => x.AddValidator(validator));
        compositeValidator.Validate(new ValidationContext<string>(null!, string.Empty, new List<string>()));
 
        validator.Received(1);
    }
}