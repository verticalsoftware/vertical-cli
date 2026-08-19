using System.Reflection;

namespace Vertical.Cli.Utilities;

internal static class ReflectionExtensions
{
    extension(Type type)
    {
        public IEnumerable<Type> GetInterfacesAndSelf()
        {
            var stack = new Stack<Type>([type]);
            var visited = new HashSet<Type>();

            while (stack.TryPop(out var current))
            {
                yield return current;

                stack.PushRange(current.GetInterfaces().Where(visited.Add));
            }
        }

        public IEnumerable<PropertyInfo> GetAllProperties(BindingFlags bindingFlags = 
            BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public)
        {
            return type
                .GetInterfacesAndSelf()
                .SelectMany(type => type.GetProperties(bindingFlags));
        }
    }
}