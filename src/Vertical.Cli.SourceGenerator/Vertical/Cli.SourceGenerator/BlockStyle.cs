namespace Vertical.Cli.SourceGenerator;

public sealed class BlockStyle
{
    private BlockStyle(
        bool finalizeOuterBlock,
        char? openingChar,
        bool newLineAfterOpeningChar,
        bool finalizeInnerBlock,
        char? closingChar,
        bool newLineAfterClosingChar)
    {
        FinalizeOuterBlock = finalizeOuterBlock;
        OpeningChar = openingChar;
        NewLineAfterOpeningChar = newLineAfterOpeningChar;
        FinalizeInnerBlock = finalizeInnerBlock;
        ClosingChar = closingChar;
        NewLineAfterClosingChar = newLineAfterClosingChar;
    }

    public bool FinalizeOuterBlock { get; }

    public char? OpeningChar { get; }

    public bool NewLineAfterOpeningChar { get; }

    public bool FinalizeInnerBlock { get; }

    public char? ClosingChar { get; }

    public bool NewLineAfterClosingChar { get; }

    public static readonly BlockStyle ClassBody = new(
        finalizeOuterBlock: true,
        openingChar: '{',
        newLineAfterOpeningChar: true,
        finalizeInnerBlock: true,
        closingChar: '}',
        newLineAfterClosingChar: true);
    
    public static readonly BlockStyle BracedBody = new(
        finalizeOuterBlock: true,
        openingChar: '{',
        newLineAfterOpeningChar: true,
        finalizeInnerBlock: true,
        closingChar: '}',
        newLineAfterClosingChar: true);
    
    public static readonly BlockStyle PropertyAssignmentBody = new(
        finalizeOuterBlock: true,
        openingChar: '{',
        newLineAfterOpeningChar: true,
        finalizeInnerBlock: true,
        closingChar: '}',
        newLineAfterClosingChar: false);
    
    public static readonly BlockStyle MethodBody = new(
        finalizeOuterBlock: true,
        openingChar: '{',
        newLineAfterOpeningChar: true,
        finalizeInnerBlock: true,
        closingChar: '}',
        newLineAfterClosingChar: true);

    public static readonly BlockStyle ParameterList = new(
        finalizeOuterBlock: false,
        openingChar: '(',
        newLineAfterOpeningChar: true,
        finalizeInnerBlock: false,
        closingChar: ')',
        newLineAfterClosingChar: false);
}