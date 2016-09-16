using System;
using System.Linq;
using System.Reflection;

namespace ServiceBridge
{
    internal static class ReflectionHelper
    {
        /// <summary>
        /// Given a MethodInfo for a property's get or set method,
        /// return the corresponding property info.
        /// </summary>
        /// <param name="method">MethodBase for the property's get or set method.</param>
        /// <returns>PropertyInfo for the property, or null if method is not part of a property.</returns>
        public static PropertyInfo GetPropertyFromMethod(MethodInfo method)
        {
            if (method == null || !method.IsSpecialName) return null;
            var containingType = method.DeclaringType;
            if (containingType == null) return null;
            var isGetter = method.Name.StartsWith("get_", StringComparison.Ordinal);
            var isSetter = method.Name.StartsWith("set_", StringComparison.Ordinal);
            if (!isSetter && !isGetter) return null;
            var propertyName = method.Name.Substring(4);
            Type propertyType;
            Type[] indexerTypes;

            GetPropertyTypes(method, isGetter, out propertyType, out indexerTypes);
#if NetCore
            return containingType.GetTypeInfo().DeclaredProperties.SingleOrDefault(p =>
            {
                if (p.Name != propertyName || p.PropertyType != propertyType) return false;
                var parameters = p.GetIndexParameters();
                if (parameters.Length != indexerTypes.Length) return false;
                for (int i = 0; i < indexerTypes.Length; i++)
                {
                    if (parameters[i].ParameterType != indexerTypes[i])
                    {
                        return false;
                    }
                }
                return true;
            });
#else
            return containingType.GetProperty(
                propertyName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly,
                null,
                propertyType,
                indexerTypes,
                null);
#endif
        }

        private static void GetPropertyTypes(MethodInfo method, bool isGetter, out Type propertyType, out Type[] indexerTypes)
        {
            var parameters = method.GetParameters();
            if (isGetter)
            {
                propertyType = method.ReturnType;
                indexerTypes =
                    parameters.Length == 0
                        ? new Type[0]
                        : parameters.Select(pi => pi.ParameterType).ToArray();
            }
            else
            {
                propertyType = parameters[parameters.Length - 1].ParameterType;
                indexerTypes =
                    parameters.Length == 1
                        ? new Type[0]
                        : parameters.Take(parameters.Length - 1).Select(pi => pi.ParameterType).ToArray();
            }
        }
    }
}
