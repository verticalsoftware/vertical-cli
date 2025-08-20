using System.Linq.Expressions;

namespace Vertical.Cli.Internal;

internal static class TypeUtilities
{
    private static readonly Type[] ExcludedBaseTypes =
    [         
        typeof(object),
        typeof(IEquatable<>),
        typeof(IComparable<>),
        typeof(IEnumerable<>),
        typeof(IParsable<>)
    ];
    
    public static IEnumerable<Type> GetSelfAndAllBaseTypes(this Type type)
    {
        var stack = new Stack<Type>([type]);
        HashSet<Type> typeSet =
        [
            typeof(object)
        ];

        while (stack.TryPop(out var target))
        {
            yield return target;

            TryPush(target.BaseType);

            foreach (var interfaceType in target.GetInterfaces())
            {
                TryPush(interfaceType);
            }
        }

        yield break;

        void TryPush(Type? baseType)
        {
            if (baseType == null)
                return;

            if (!typeSet.Add(baseType))
                return;

            if (baseType.IsGenericType && ExcludedBaseTypes.Contains(baseType.GetGenericTypeDefinition()))
                return;
            
            stack.Push(baseType);
        }
    }

    public static string GetPropertyName<TModel, TValue>(this Expression<Func<TModel, TValue>> expression)
    {
        return ((MemberExpression)expression.Body).Member.Name;
    }
}