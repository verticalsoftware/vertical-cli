using System.Text;
using Shouldly;

namespace Vertical.Cli.Parsing;

public class ResponseFilePreProcessorTests
{
    [Fact]
    public void Injects_Single_Response_File()
    {
        string[] file =
        [
            "-a",
            "file1.txt file2.txt",
            "--user-id=admin"
        ];
        var args = GetResult(["@file"], _ => MakeStream(file));
        
        args.ShouldBe([
            "-a",
            "file1.txt",
            "file2.txt",
            "--user-id=admin"
        ]);
    }

    [Fact]
    public void Injects_Multiple_Response_Files()
    {
        var files = new Queue<string[]>([
            ["--user-id=admin", "-p", "(secret)"],
            ["--verbosity", "info", "input1.txt", "input2.txt input3.txt"]
        ]);

        var args = GetResult(["@file1.rsp", "@file2.rsp"], _ => MakeStream(files.Dequeue()));
        
        args.ShouldBe([
            "--user-id=admin", 
            "-p", 
            "(secret)",
            "--verbosity", 
            "info", 
            "input1.txt", 
            "input2.txt", 
            "input3.txt"
        ]);
    }

    [Fact]
    public void Ignores_Comments()
    {
        string[] file =
        [
            "# Declare a user id",
            "--user-id",
            "admin",
            "--password",
            "(secret) # not a real secret"
        ];

        var args = GetResult(["@file.rsp"], _ => MakeStream(file));
        
        args.ShouldBe([
            "--user-id",
            "admin",
            "--password",
            "(secret)"
        ]);
    }

    [Fact]
    public void Injects_Quoted_String()
    {
        var args = GetResult(["@file.rsp"], _ => MakeStream([
            "-a",
            "\"now is the time\"",
            "-b"
        ]));

        args.ShouldBe([
            "-a",
            "now is the time",
            "-b"
        ]);
    }

    private static IEnumerable<string> GetResult(string[] args, Func<FileInfo, Stream> streamProvider)
    {
        var list = new LinkedList<string>(args);
        ResponseFilePreProcessor.Handle(
            list,
            streamProvider);

        return list;
    }

    private static MemoryStream MakeStream(string[] lines)
    {
        var stream = new MemoryStream();
        foreach (var line in lines)
        {
            stream.Write(Encoding.UTF8.GetBytes(line));
            stream.Write(Encoding.UTF8.GetBytes(Environment.NewLine));
        }

        stream.Position = 0;
        return stream;
    }
}