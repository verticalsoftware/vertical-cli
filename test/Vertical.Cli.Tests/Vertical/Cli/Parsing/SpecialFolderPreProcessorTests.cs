using Shouldly;

namespace Vertical.Cli.Parsing;

public class SpecialFolderPreProcessorTests
{
    [Fact]
    public void Replaces_With_Special_Folder_Value()
    {
        var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var list = new LinkedList<string>(["--path=$(SpecialFolder.LocalApplicationData)"]);

        SpecialFolderPreProcessor.Handle(list);
        
        list.Single().ShouldBe($"--path={localAppData}");
    }

    [Fact]
    public void Ignores_Invalid_Enum_Constant()
    {
        var list = new LinkedList<string>(["--path=$(SpecialFolder.Unknown)"]);
        SpecialFolderPreProcessor.Handle(list);
        
        list.Single().ShouldBe("--path=$(SpecialFolder.Unknown)");
    }
}