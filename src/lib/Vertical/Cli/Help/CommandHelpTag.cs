namespace Vertical.Cli.Help;

/// <summary>
/// Defines structured help content for a command.
/// </summary>
/// <param name="Description">Description of the command</param>
/// <param name="IntroductoryRemarks">Optional content written immediately after the description.</param>
/// <param name="FinalRemarks">Optional content written at the end of the help topic.</param>
public record CommandHelpTag(
    string Description,
    RemarksSection? IntroductoryRemarks,
    RemarksSection? FinalRemarks);
