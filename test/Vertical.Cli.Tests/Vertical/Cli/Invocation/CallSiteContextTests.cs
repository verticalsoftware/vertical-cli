using NSubstitute;
using Vertical.Cli.Binding;
using Vertical.Cli.Help;

namespace Vertical.Cli.Invocation;

public class CallSiteContextTests
{
    public enum Compression
    {
        None,
        GZip
    }

    public record CopyModel(
        string Source,
        string Dest,
        bool Overwrite,
        Compression Compression);

    public record DeleteModel(
        string Source,
        bool NoConfirm);

    private static readonly ModelBinder<CopyModel> CopyModelBinder = new DelegatedBinder<CopyModel>(
        context => new CopyModel(
            context.GetValue<string>("source"),
            context.GetValue<string>("dest"),
            context.GetValue<bool>("--overwrite"),
            context.GetValue<Compression>("--compression")));

    private static readonly ModelBinder<DeleteModel> DeleteModelBinder = new DelegatedBinder<DeleteModel>(
        context => new DeleteModel(
            context.GetValue<string>("source"),
            context.GetValue<bool>("--no-confirm")));

    [Fact]
    public async Task CopyCommandSiteInvoked()
    {
        var handler = new Func<CopyModel, Task<int>>(async model =>
        {
            Assert.Equal("/usr/source.txt", model.Source);
            Assert.Equal("/usr/source.copy.txt", model.Dest);
            Assert.Equal(Compression.GZip, model.Compression);
            Assert.True(model.Overwrite);
            await Task.CompletedTask;
            return 1;
        });

        var root = SetupRoot(copyHandler: handler);
        var context = CallSiteContext.Create(root, new[]
        {
            "cp",
            "--compression=gzip",
            "--overwrite",
            "/usr/source.txt",
            "/usr/source.copy.txt"
        }, Task.FromResult(-1));
        
        Assert.Equal("cp", context.CallSite.Subject.Id);
        Assert.Equal(typeof(CopyModel), context.CallSite.ModelType);
        Assert.Equal(CallState.Command, context.CallSite.State);

        var call = context.BindModelToCallSite<CopyModel>();
        var result = await call(CancellationToken.None);
        Assert.Equal(1, result);
    }

    [Fact]
    public async Task RemoveCommandSiteInvoked()
    {
        var handler = new Func<DeleteModel, Task<int>>(async model =>
        {
            Assert.Equal("/usr/source.txt", model.Source);
            Assert.True(model.NoConfirm);
            await Task.CompletedTask;
            return 1;
        });

        var root = SetupRoot(removeHandler: handler);
        var context = CallSiteContext.Create(root, new[]
        {
            "rm",
            "/usr/source.txt",
            "--no-confirm"
        }, Task.FromResult(-1));
        
        Assert.Equal("rm", context.CallSite.Subject.Id);
        Assert.Equal(typeof(DeleteModel), context.CallSite.ModelType);
        Assert.Equal(CallState.Command, context.CallSite.State);

        var call = context.BindModelToCallSite<DeleteModel>();
        var result = await call(CancellationToken.None);
        Assert.Equal(1, result);
    }

    public record BindModel;
    
    [Fact]
    public void BindingDelegateInvoked()
    {
        var root = RootCommand.Create<BindModel, int>("root", cmd => cmd.SetHandler(_ => 0));
        var context = CallSiteContext.Create(root, Array.Empty<string>(), 0);
        var called = false;
        _ = context.BindModelToCallSite<BindModel>(_ =>
        {
            called = true;
            return new BindModel();
        });
        
        Assert.True(called);
    }

    [Fact]
    public void BindingDelegateNotCalledForNoneModelType()
    {
        var root = RootCommand.Create<int>("root", cmd => cmd.SetHandler(_ => 0));
        var context = CallSiteContext.Create(root, Array.Empty<string>(), 0);
        var called = false;
        _ = context.BindModelToCallSite<None>(_ =>
        {
            called = true;
            return None.Default;
        });
        
        Assert.False(called);
    }

    [Fact]
    public void HelpSiteSelected()
    {
        var helpFormatter = Substitute.For<IHelpFormatter>();
        var root = RootCommand.Create<int>("root", cmd =>
        {
            cmd.SetHandler(_ => 0);
            cmd.SetHelpOption(formatterProvider: () => helpFormatter);
        });
        var context = CallSiteContext.Create(root, new[] { "--help" }, 0);
        
        Assert.Equal(CallState.Help, context.CallSite.State);
        context.BindModelToCallSite<None>(_ => None.Default)(CancellationToken.None);
        helpFormatter.Received();
    }

    private static IRootCommand<None, Task<int>> SetupRoot(
        Func<CopyModel, Task<int>>? copyHandler = null,
        Func<DeleteModel, Task<int>>? removeHandler = null)
    {
        var options = new CliOptions();
        options.AddBinder(() => CopyModelBinder);
        options.AddBinder(() => DeleteModelBinder);
        
        return RootCommand.Create<Task<int>>(
            "root",
            root =>
            {
                root.AddArgument<string>("source", scope: SymbolScope.Descendents);

                root.ConfigureSubCommand<CopyModel>("cp", cmd =>
                {
                    cmd.AddArgument<string>("dest")
                        .AddSwitch("--overwrite")
                        .AddOption("--compression", defaultProvider: () => Compression.None);

                    cmd.SetHandler(async model => copyHandler != null
                        ? await copyHandler(model)
                        : 0);
                });

                root.ConfigureSubCommand<DeleteModel>("rm", cmd =>
                {
                    cmd.AddSwitch("--no-confirm");

                    cmd.SetHandler(async model => removeHandler != null
                        ? await removeHandler(model)
                        : 0);
                });
            }, options);
    }
}