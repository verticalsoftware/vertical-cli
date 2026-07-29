using System.Linq.Expressions;
using System.Reflection;

namespace Vertical.Cli.Utilities;

internal static class ExpressionExtensions
{
    extension<TModel, TValue>(Expression<Func<TModel, TValue>> expression)
    {
        public string BindingName => expression.PropertyInfo.Name;

        public PropertyInfo PropertyInfo
        {
            get
            {
                if (expression.Body is not MemberExpression memberExpression)
                {
                    throw new InvalidOperationException($"{expression} body is not a member expression.");
                }
                
                return memberExpression.Member as PropertyInfo
                    ?? throw new InvalidOperationException($"{expression} member is not a property.");
            }   
        }
    }
}