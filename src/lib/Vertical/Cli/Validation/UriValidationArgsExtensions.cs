namespace Vertical.Cli.Validation;

public static partial class ValidationArgsExtensions
{
    /// <summary>
    /// Adds a validation action that reports an error if the provided value is not an absolute URI.
    /// </summary>
    /// <param name="context"><see cref="ValidationContext{TModel,TValue}"/></param>
    /// <param name="messageProvider">A function that provides a custom message when the validation fails.</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <typeparam name="TValue">File system value type.</typeparam>
    /// <returns>A reference to this provided <see cref="ValidationContext{TModel,TValue}"/> instance.</returns>
    public static ValidationContext<TModel, Uri> IsAbsoluteUri<TModel, TValue>(
        this ValidationContext<TModel, Uri> context,
        Func<Uri, string>? messageProvider = null) 
        where TModel : class
        where TValue : FileSystemInfo
    {
        ArgumentNullException.ThrowIfNull(context);

        if (context.Value.IsAbsoluteUri)
            return context;
        
        context.AddValidationError("URI must be an absolute URI");
        return context;
    }
    
    /// <summary>
    /// Adds a validation action that reports an error if the provided value is not a relative URI.
    /// </summary>
    /// <param name="context"><see cref="ValidationContext{TModel,TValue}"/></param>
    /// <param name="messageProvider">A function that provides a custom message when the validation fails.</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <typeparam name="TValue">File system value type.</typeparam>
    /// <returns>A reference to this provided <see cref="ValidationContext{TModel,TValue}"/> instance.</returns>
    public static ValidationContext<TModel, Uri> IsRelativeUri<TModel, TValue>(
        this ValidationContext<TModel, Uri> context,
        Func<Uri, string>? messageProvider = null) 
        where TModel : class
        where TValue : FileSystemInfo
    {
        ArgumentNullException.ThrowIfNull(context);
        
        if (!context.Value.IsAbsoluteUri)
            return context;
        
        context.AddValidationError("URI must be a relative URI");
        return context;
    }

    /// <summary>
    /// Adds a validation action that reports an error if the URIs scheme is not one of the provided values.
    /// </summary>
    /// <param name="context"><see cref="ValidationContext{TModel,TValue}"/></param>
    /// <param name="schemes">The valid schemes to accept.</param>
    /// <param name="messageProvider">A function that provides a custom message when the validation fails.</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <typeparam name="TValue">File system value type.</typeparam>
    /// <returns>A reference to this provided <see cref="ValidationContext{TModel,TValue}"/> instance.</returns>
    public static ValidationContext<TModel, Uri> HasScheme<TModel, TValue>(
        this ValidationContext<TModel, Uri> context,
        IEnumerable<string> schemes,
        Func<Uri, string>? messageProvider = null) 
        where TModel : class
        where TValue : FileSystemInfo
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(schemes);
        
        if (schemes.Contains(context.Value.Scheme))
            return context;

        var schemeList = string.Join(", ", schemes);
        context.AddValidationError($"URI must have one of the following schemes: [{schemeList}]");
        return context;
    }
    
    /// <summary>
    /// Adds a validation action that reports an error if the URIs host is not one of the provided values.
    /// </summary>
    /// <param name="context"><see cref="ValidationContext{TModel,TValue}"/></param>
    /// <param name="hosts">The valid hosts to accept.</param>
    /// <param name="messageProvider">A function that provides a custom message when the validation fails.</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <typeparam name="TValue">File system value type.</typeparam>
    /// <returns>A reference to this provided <see cref="ValidationContext{TModel,TValue}"/> instance.</returns>
    public static ValidationContext<TModel, Uri> HasHost<TModel, TValue>(
        this ValidationContext<TModel, Uri> context,
        IEnumerable<string> hosts,
        Func<Uri, string>? messageProvider = null) 
        where TModel : class
        where TValue : FileSystemInfo
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(hosts);
        
        if (hosts.Contains(context.Value.Host, StringComparer.OrdinalIgnoreCase))
            return context;

        var valueList = string.Join(", ", hosts);
        context.AddValidationError($"URI must have one of the following hosts: [{valueList}]");
        return context;
    }
    
    /// <summary>
    /// Adds a validation action that reports an error if the URIs port is not one of the provided values.
    /// </summary>
    /// <param name="context"><see cref="ValidationContext{TModel,TValue}"/></param>
    /// <param name="ports">The valid hosts to accept.</param>
    /// <param name="messageProvider">A function that provides a custom message when the validation fails.</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <typeparam name="TValue">File system value type.</typeparam>
    /// <returns>A reference to this provided <see cref="ValidationContext{TModel,TValue}"/> instance.</returns>
    public static ValidationContext<TModel, Uri> HasPort<TModel, TValue>(
        this ValidationContext<TModel, Uri> context,
        IEnumerable<int> ports,
        Func<Uri, string>? messageProvider = null) 
        where TModel : class
        where TValue : FileSystemInfo
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(ports);
        
        if (ports.Contains(context.Value.Port))
            return context;

        var valueList = string.Join(", ", ports);
        context.AddValidationError($"URI must have one of the following ports: [{valueList}]");
        return context;
    }

    /// <summary>
    /// Adds a validation action that reports an error if the URIs port is not in an inclusive range.
    /// </summary>
    /// <param name="context"><see cref="ValidationContext{TModel,TValue}"/></param>
    /// <param name="highestPort">The highest acceptable port.</param>
    /// <param name="messageProvider">A function that provides a custom message when the validation fails.</param>
    /// <param name="lowestPort">The lower acceptable port.</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <typeparam name="TValue">File system value type.</typeparam>
    /// <returns>A reference to this provided <see cref="ValidationContext{TModel,TValue}"/> instance.</returns>
    public static ValidationContext<TModel, Uri> HasPortRange<TModel, TValue>(
        this ValidationContext<TModel, Uri> context,
        int lowestPort,
        int highestPort,
        Func<Uri, string>? messageProvider = null) 
        where TModel : class
        where TValue : FileSystemInfo
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentOutOfRangeException.ThrowIfLessThan(lowestPort, 0);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(highestPort, ushort.MaxValue);
        
        if (context.Value.Port >= lowestPort && context.Value.Port <= highestPort)
            return context;

        context.AddValidationError($"URI port must be in the range of {lowestPort}-{highestPort}");
        return context;
    }
    
    /// <summary>
    /// Adds a validation action that reports an error if the provided value is not null and an absolute URI.
    /// </summary>
    /// <param name="context"><see cref="ValidationContext{TModel,TValue}"/></param>
    /// <param name="messageProvider">A function that provides a custom message when the validation fails.</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <typeparam name="TValue">File system value type.</typeparam>
    /// <returns>A reference to this provided <see cref="ValidationContext{TModel,TValue}"/> instance.</returns>
    public static ValidationContext<TModel, Uri?> IsAbsoluteUriOrIsNull<TModel, TValue>(
        this ValidationContext<TModel, Uri?> context,
        Func<Uri, string>? messageProvider = null) 
        where TModel : class
        where TValue : FileSystemInfo
    {
        ArgumentNullException.ThrowIfNull(context);
        
        if (context.Value is null || context.Value.IsAbsoluteUri)
            return context;
        
        context.AddValidationError("URI must be an absolute URI");
        return context;
    }
    
    /// <summary>
    /// Adds a validation action that reports an error if the provided value is not null and not a relative URI.
    /// </summary>
    /// <param name="context"><see cref="ValidationContext{TModel,TValue}"/></param>
    /// <param name="messageProvider">A function that provides a custom message when the validation fails.</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <typeparam name="TValue">File system value type.</typeparam>
    /// <returns>A reference to this provided <see cref="ValidationContext{TModel,TValue}"/> instance.</returns>
    public static ValidationContext<TModel, Uri?> IsRelativeUriOrIsNull<TModel, TValue>(
        this ValidationContext<TModel, Uri?> context,
        Func<Uri, string>? messageProvider = null) 
        where TModel : class
        where TValue : FileSystemInfo
    {
        ArgumentNullException.ThrowIfNull(context);
        
        if (context.Value is null || !context.Value.IsAbsoluteUri)
            return context;
        
        context.AddValidationError("URI must be a relative URI");
        return context;
    }

    /// <summary>
    /// Adds a validation action that reports an error if the URI is not null and its scheme is not one of the provided values.
    /// </summary>
    /// <param name="context"><see cref="ValidationContext{TModel,TValue}"/></param>
    /// <param name="schemes">The valid schemes to accept.</param>
    /// <param name="messageProvider">A function that provides a custom message when the validation fails.</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <typeparam name="TValue">File system value type.</typeparam>
    /// <returns>A reference to this provided <see cref="ValidationContext{TModel,TValue}"/> instance.</returns>
    public static ValidationContext<TModel, Uri?> HasSchemeOrIsNull<TModel, TValue>(
        this ValidationContext<TModel, Uri?> context,
        IEnumerable<string> schemes,
        Func<Uri, string>? messageProvider = null) 
        where TModel : class
        where TValue : FileSystemInfo
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(schemes);
        
        if (context.Value is null || schemes.Contains(context.Value.Scheme))
            return context;

        var schemeList = string.Join(", ", schemes);
        context.AddValidationError($"URI must have one of the following schemes: [{schemeList}]");
        return context;
    }
    
    /// <summary>
    /// Adds a validation action that reports an error if the URI is not null and its host is not one of the provided values.
    /// </summary>
    /// <param name="context"><see cref="ValidationContext{TModel,TValue}"/></param>
    /// <param name="hosts">The valid hosts to accept.</param>
    /// <param name="messageProvider">A function that provides a custom message when the validation fails.</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <typeparam name="TValue">File system value type.</typeparam>
    /// <returns>A reference to this provided <see cref="ValidationContext{TModel,TValue}"/> instance.</returns>
    public static ValidationContext<TModel, Uri?> HasHostOrIsNull<TModel, TValue>(
        this ValidationContext<TModel, Uri?> context,
        IEnumerable<string> hosts,
        Func<Uri, string>? messageProvider = null) 
        where TModel : class
        where TValue : FileSystemInfo
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(hosts);
        
        if (context.Value is null || hosts.Contains(context.Value.Host, StringComparer.OrdinalIgnoreCase))
            return context;

        var valueList = string.Join(", ", hosts);
        context.AddValidationError($"URI must have one of the following hosts: [{valueList}]");
        return context;
    }
    
    /// <summary>
    /// Adds a validation action that reports an error if the URI is not null and its port is not one of the provided values.
    /// </summary>
    /// <param name="context"><see cref="ValidationContext{TModel,TValue}"/></param>
    /// <param name="ports">The valid hosts to accept.</param>
    /// <param name="messageProvider">A function that provides a custom message when the validation fails.</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <typeparam name="TValue">File system value type.</typeparam>
    /// <returns>A reference to this provided <see cref="ValidationContext{TModel,TValue}"/> instance.</returns>
    public static ValidationContext<TModel, Uri?> HasPortOrIsNull<TModel, TValue>(
        this ValidationContext<TModel, Uri?> context,
        IEnumerable<int> ports,
        Func<Uri, string>? messageProvider = null) 
        where TModel : class
        where TValue : FileSystemInfo
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(ports);
        
        if (context.Value is null || ports.Contains(context.Value.Port))
            return context;

        var valueList = string.Join(", ", ports);
        context.AddValidationError($"URI must have one of the following ports: [{valueList}]");
        return context;
    }

    /// <summary>
    /// Adds a validation action that reports an error if the URI and not null and its port is not in an inclusive range.
    /// </summary>
    /// <param name="context"><see cref="ValidationContext{TModel,TValue}"/></param>
    /// <param name="highestPort">The highest acceptable port.</param>
    /// <param name="messageProvider">A function that provides a custom message when the validation fails.</param>
    /// <param name="lowestPort">The lower acceptable port.</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <typeparam name="TValue">File system value type.</typeparam>
    /// <returns>A reference to this provided <see cref="ValidationContext{TModel,TValue}"/> instance.</returns>
    public static ValidationContext<TModel, Uri?> HasPortRangeOrIsNull<TModel, TValue>(
        this ValidationContext<TModel, Uri?> context,
        int lowestPort,
        int highestPort,
        Func<Uri, string>? messageProvider = null) 
        where TModel : class
        where TValue : FileSystemInfo
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentOutOfRangeException.ThrowIfLessThan(lowestPort, 0);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(highestPort, ushort.MaxValue);
        
        if (context.Value is null || context.Value.Port >= lowestPort && context.Value.Port <= highestPort)
            return context;

        context.AddValidationError($"URI port must be in the range of {lowestPort}-{highestPort}");
        return context;
    }
}