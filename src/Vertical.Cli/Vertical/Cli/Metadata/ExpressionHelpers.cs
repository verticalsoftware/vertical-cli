using System.Linq.Expressions;

namespace Vertical.Cli.Metadata;

internal static class ExpressionHelpers
{
    internal static string GetMemberName<TModel, TValue>(this Expression<Func<TModel, TValue>> expression)
    {
        return
            (expression.Body as MemberExpression)?.Member.Name
            ??
            throw new ArgumentException($"Cannot evaluate {expression} as a member binding expression");
    }
}