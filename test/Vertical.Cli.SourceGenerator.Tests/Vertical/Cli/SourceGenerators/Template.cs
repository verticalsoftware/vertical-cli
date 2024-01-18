/*
using System;
using System.Threading.Tasks;
 
namespace Vertical.Cli
{
    namespace Configuration
    {
        public interface IRootCommandBuilder<TModel, TResult>
        {
        }
    }
    
    public interface IRootCommand<TModel, TResult>
    {
    }

    public static class RootCommand
    {
        public static IRootCommand<TModel, TResult> Create<TModel, TResult>(
            string id,
            Action<Configuration.IRootCommandBuilder<TModel, TResult>> configure) => null!;
    }
}

namespace TestSetup
{
    public enum Compression { None, GZip }

    public record FileCopyParameters(
        FileInfo Source,
        FileInfo Dest,
        Compression Compression,
        bool Overwrite);

    public static class Program
    {
        public static System.Threading.Tasks.Task<int> Main(string[] args)
        {
            _ = Vertical.Cli.RootCommand.Create<FileCopyParameters, Task<int>>(
                id: "copy",
                root => { });

            return Task.FromResult(0);
        }
    }
}
*/
