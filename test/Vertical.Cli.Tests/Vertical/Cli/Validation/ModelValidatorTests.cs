using NSubstitute;
using Shouldly;
using Vertical.Cli.Configuration;

namespace Vertical.Cli.Validation;

public class ModelValidatorTests
{
    public interface Evaluator
    {
        ValidationResult Receive(Model<string> model, string value);
    }
    
    [Fact]
    public void Evaluators_Invoked()
    {
        var command = new RootCommand<Model<string>, int>("root");
        command.AddArgument(x => x.Value);
        var symbol = command.Symbols.First();
        
        Evaluator[] evaluators =
        [
            Substitute.For<Evaluator>(),
            Substitute.For<Evaluator>()
        ];
        
        var unit = new ModelValidator<Model<string>>();
        var model = new Model<string>("string");

        evaluators[0].Receive(model, "string").Returns(ValidationResult.Ok);
        evaluators[1].Receive(model, "string").Returns(ValidationResult.Ok);
        
        unit.AddEvaluation(symbol, m => m.Value, evaluators[0].Receive);
        unit.AddEvaluation(symbol, m => m.Value, evaluators[1].Receive);

        unit.Validate(model, new ValidationContext());

        evaluators[0].Received().Receive(model, "string");
        evaluators[1].Received().Receive(model, "string");
    }

    [Fact]
    public void Validate_Adds_Errors()
    {
        var command = new RootCommand<Model<string>, int>("root");
        command.AddArgument(x => x.Value, validation: b => b.HasMinLength(3));

        var context = new ValidationContext();
        command.ValidateModel(new Model<string>("ab"), context);

        context.IsValid.ShouldBeFalse();
        context.Errors.Count.ShouldBe(1);
    }
}