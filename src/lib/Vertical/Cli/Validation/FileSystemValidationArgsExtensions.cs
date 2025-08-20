namespace Vertical.Cli.Validation;

public static partial class ValidationArgsExtensions
{
    /// <summary>
    /// Adds a validation action that reports an error if the provided file system object does not exist.
    /// </summary>
    /// <param name="context"><see cref="ValidationContext{TModel,TValue}"/></param>
    /// <param name="messageProvider">A function that provides a custom message when the validation fails.</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <typeparam name="TValue">File system value type.</typeparam>
    /// <returns>A reference to this provided <see cref="ValidationContext{TModel,TValue}"/> instance.</returns>
    public static ValidationContext<TModel, TValue> Exists<TModel, TValue>(
        this ValidationContext<TModel, TValue> context,
        Func<TValue, string>? messageProvider = null) 
        where TModel : class
        where TValue : FileSystemInfo
    {
        ArgumentNullException.ThrowIfNull(context);
        
        if (context.Value.Exists)
            return context;

        var typeString = context.Value switch
        {
            DirectoryInfo => "directory",
            FileInfo => "file",
            _ => throw new InvalidOperationException()
        };
        
        context.AddValidationError($"{typeString} does not exist");
        return context;
    }
    
    /// <summary>
    /// Adds a validation action that reports an error if the provided directory does not exist.
    /// </summary>
    /// <param name="context"><see cref="ValidationContext{TModel,TValue}"/></param>
    /// <param name="messageProvider">A function that provides a custom message when the validation fails.</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <returns>A reference to this provided <see cref="ValidationContext{TModel,TValue}"/> instance.</returns>
    public static ValidationContext<TModel, DirectoryInfo> Exists<TModel>(
        this ValidationContext<TModel, DirectoryInfo> context,
        Func<DirectoryInfo, string>? messageProvider = null) 
        where TModel : class
    {
        ArgumentNullException.ThrowIfNull(context);
        
        if (context.Value.Exists)
            return context;

        context.AddValidationError("directory does not exist");
        return context;
    }
    
    /// <summary>
    /// Adds a validation action that reports an error if the provided file does not exist.
    /// </summary>
    /// <param name="context"><see cref="ValidationContext{TModel,TValue}"/></param>
    /// <param name="messageProvider">A function that provides a custom message when the validation fails.</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <returns>A reference to this provided <see cref="ValidationContext{TModel,TValue}"/> instance.</returns>
    public static ValidationContext<TModel, FileInfo> Exists<TModel>(
        this ValidationContext<TModel, FileInfo> context,
        Func<FileInfo, string>? messageProvider = null) 
        where TModel : class
    {
        ArgumentNullException.ThrowIfNull(context);
        
        if (context.Value.Exists)
            return context;

        context.AddValidationError("file does not exist");
        return context;
    }
    
    /// <summary>
    /// Adds a validation action that reports an error if the provided file system object exists.
    /// </summary>
    /// <param name="context"><see cref="ValidationContext{TModel,TValue}"/></param>
    /// <param name="messageProvider">A function that provides a custom message when the validation fails.</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <typeparam name="TValue">File system value type.</typeparam>
    /// <returns>A reference to this provided <see cref="ValidationContext{TModel,TValue}"/> instance.</returns>
    public static ValidationContext<TModel, TValue> DoesNotExist<TModel, TValue>(
        this ValidationContext<TModel, TValue> context,
        Func<TValue, string>? messageProvider = null) 
        where TModel : class
        where TValue : FileSystemInfo
    {
        ArgumentNullException.ThrowIfNull(context);
        
        if (!context.Value.Exists)
            return context;

        var typeString = context.Value switch
        {
            DirectoryInfo => "directory",
            FileInfo => "file",
            _ => throw new InvalidOperationException()
        };
        
        context.AddValidationError($"{typeString} already created");
        return context;
    }
    
    /// <summary>
    /// Adds a validation action that reports an error if the provided directory exists.
    /// </summary>
    /// <param name="context"><see cref="ValidationContext{TModel,TValue}"/></param>
    /// <param name="messageProvider">A function that provides a custom message when the validation fails.</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <returns>A reference to this provided <see cref="ValidationContext{TModel,TValue}"/> instance.</returns>
    public static ValidationContext<TModel, DirectoryInfo> DoesNotExist<TModel>(
        this ValidationContext<TModel, DirectoryInfo> context,
        Func<DirectoryInfo, string>? messageProvider = null) 
        where TModel : class
    {
        ArgumentNullException.ThrowIfNull(context);
        
        if (!context.Value.Exists)
            return context;

        context.AddValidationError("directory already created");
        return context;
    }
    
    /// <summary>
    /// Adds a validation action that reports an error if the provided file exists.
    /// </summary>
    /// <param name="context"><see cref="ValidationContext{TModel,TValue}"/></param>
    /// <param name="messageProvider">A function that provides a custom message when the validation fails.</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <returns>A reference to this provided <see cref="ValidationContext{TModel,TValue}"/> instance.</returns>
    public static ValidationContext<TModel, FileInfo> DoesNotExist<TModel>(
        this ValidationContext<TModel, FileInfo> context,
        Func<FileInfo, string>? messageProvider = null) 
        where TModel : class
    {
        ArgumentNullException.ThrowIfNull(context);
        
        if (!context.Value.Exists)
            return context;

        context.AddValidationError("file already created");
        return context;
    }
    
    /// <summary>
    /// Adds a validation action that reports an error if the provided file system object is not null and does not exist.
    /// </summary>
    /// <param name="context"><see cref="ValidationContext{TModel,TValue}"/></param>
    /// <param name="messageProvider">A function that provides a custom message when the validation fails.</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <typeparam name="TValue">File system value type.</typeparam>
    /// <returns>A reference to this provided <see cref="ValidationContext{TModel,TValue}"/> instance.</returns>
    public static ValidationContext<TModel, TValue?> ExistsOrIsNull<TModel, TValue>(
        this ValidationContext<TModel, TValue?> context,
        Func<TValue, string>? messageProvider = null) 
        where TModel : class
        where TValue : FileSystemInfo
    {
        ArgumentNullException.ThrowIfNull(context);
        
        if (context.Value is null || context.Value.Exists)
            return context;

        var typeString = context.Value switch
        {
            DirectoryInfo => "directory",
            FileInfo => "file",
            _ => throw new InvalidOperationException()
        };
        
        context.AddValidationError($"{typeString} does not exist");
        return context;
    }
    
    /// <summary>
    /// Adds a validation action that reports an error if the provided directory is not null and does not exist.
    /// </summary>
    /// <param name="context"><see cref="ValidationContext{TModel,TValue}"/></param>
    /// <param name="messageProvider">A function that provides a custom message when the validation fails.</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <returns>A reference to this provided <see cref="ValidationContext{TModel,TValue}"/> instance.</returns>
    public static ValidationContext<TModel, DirectoryInfo?> ExistsOrIsNull<TModel>(
        this ValidationContext<TModel, DirectoryInfo?> context,
        Func<DirectoryInfo, string>? messageProvider = null) 
        where TModel : class
    {
        ArgumentNullException.ThrowIfNull(context);
        
        if (context.Value is null || context.Value.Exists)
            return context;

        context.AddValidationError("directory does not exist");
        return context;
    }
    
    /// <summary>
    /// Adds a validation action that reports an error if the provided file is not null and does not exist.
    /// </summary>
    /// <param name="context"><see cref="ValidationContext{TModel,TValue}"/></param>
    /// <param name="messageProvider">A function that provides a custom message when the validation fails.</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <returns>A reference to this provided <see cref="ValidationContext{TModel,TValue}"/> instance.</returns>
    public static ValidationContext<TModel, FileInfo?> ExistsOrIsNull<TModel>(
        this ValidationContext<TModel, FileInfo?> context,
        Func<FileInfo, string>? messageProvider = null) 
        where TModel : class
    {
        ArgumentNullException.ThrowIfNull(context);
        
        if (context.Value is null || context.Value.Exists)
            return context;

        context.AddValidationError("file does not exist");
        return context;
    }
    
    /// <summary>
    /// Adds a validation action that reports an error if the provided file system object is not null and exists.
    /// </summary>
    /// <param name="context"><see cref="ValidationContext{TModel,TValue}"/></param>
    /// <param name="messageProvider">A function that provides a custom message when the validation fails.</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <typeparam name="TValue">File system value type.</typeparam>
    /// <returns>A reference to this provided <see cref="ValidationContext{TModel,TValue}"/> instance.</returns>
    public static ValidationContext<TModel, TValue?> DoesNotExistOrIsNull<TModel, TValue>(
        this ValidationContext<TModel, TValue?> context,
        Func<TValue, string>? messageProvider = null) 
        where TModel : class
        where TValue : FileSystemInfo
    {
        ArgumentNullException.ThrowIfNull(context);
        
        if (context.Value is null || !context.Value.Exists)
            return context;

        var typeString = context.Value switch
        {
            DirectoryInfo => "directory",
            FileInfo => "file",
            _ => throw new InvalidOperationException()
        };
        
        context.AddValidationError($"{typeString} already created");
        return context;
    }
    
    /// <summary>
    /// Adds a validation action that reports an error if the provided directory is not null and exists.
    /// </summary>
    /// <param name="context"><see cref="ValidationContext{TModel,TValue}"/></param>
    /// <param name="messageProvider">A function that provides a custom message when the validation fails.</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <returns>A reference to this provided <see cref="ValidationContext{TModel,TValue}"/> instance.</returns>
    public static ValidationContext<TModel, DirectoryInfo?> DoesNotExistOrIsNull<TModel>(
        this ValidationContext<TModel, DirectoryInfo?> context,
        Func<DirectoryInfo, string>? messageProvider = null) 
        where TModel : class
    {
        ArgumentNullException.ThrowIfNull(context);
        
        if (context.Value is null || !context.Value.Exists)
            return context;

        context.AddValidationError("directory already created");
        return context;
    }
    
    /// <summary>
    /// Adds a validation action that reports an error if the provided file is not null and exists.
    /// </summary>
    /// <param name="context"><see cref="ValidationContext{TModel,TValue}"/></param>
    /// <param name="messageProvider">A function that provides a custom message when the validation fails.</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <returns>A reference to this provided <see cref="ValidationContext{TModel,TValue}"/> instance.</returns>
    public static ValidationContext<TModel, FileInfo?> DoesNotExistOrIsNull<TModel>(
        this ValidationContext<TModel, FileInfo?> context,
        Func<FileInfo, string>? messageProvider = null) 
        where TModel : class
    {
        ArgumentNullException.ThrowIfNull(context);
        
        if (context.Value is null || !context.Value.Exists)
            return context;

        context.AddValidationError("file already created");
        return context;
    }
}