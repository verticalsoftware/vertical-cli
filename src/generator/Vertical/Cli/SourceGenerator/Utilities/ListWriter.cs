namespace Vertical.Cli.SourceGenerator.Utilities;

public ref struct ListWriter(CodeFormatter builder)
{
    private int _id;

    public void WriteNext(string str)
    {
        if (_id++ > 0)
        {
            builder.WriteLine(',');
        }

        builder.Write(str);
    }

    public void WriteNext(Action<CodeFormatter> writeToFormatter)
    {
        if (_id++ > 0)
        {
            builder.WriteLine(',');
        }

        writeToFormatter(builder);
    }

    public CodeFormatter Complete()
    {
        if (_id == 0)
        {
            return builder;
        }

        builder.WriteLine();
        return builder;
    }
}