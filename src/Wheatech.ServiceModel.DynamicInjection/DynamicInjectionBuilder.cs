using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Wheatech.ServiceModel.DynamicInjection
{
    internal class DynamicInjectionBuilder
    {
        private static readonly MethodInfo _resolveDependencyMethod;
        private static readonly ConcurrentDictionary<DynamicInjectionKey, Action<IServiceContainer, object>> _cache;

        static DynamicInjectionBuilder()
        {
            _resolveDependencyMethod = typeof(IServiceContainer).GetMethod("GetInstance", BindingFlags.Instance | BindingFlags.Public, null, new[] { typeof(Type), typeof(string) }, null);
            _cache = new ConcurrentDictionary<DynamicInjectionKey, Action<IServiceContainer, object>>();
        }

        private static IEnumerable<Expression> GetParameterExpressions(ParameterExpression container, MethodInfo method)
        {
            foreach (var parameter in method.GetParameters())
            {
                yield return Expression.Convert(
                    Expression.Call(
                        Expression.Convert(container, typeof(IServiceContainer)),
                        _resolveDependencyMethod,
                        Expression.Constant(parameter.ParameterType), Expression.Constant(null, typeof(string))),
                    parameter.ParameterType);// container.GetInstance($ParameterType$, null)
            }
        }

        private static Expression GetPropertyExpression(ParameterExpression container, ParameterExpression instance, PropertyInfo property)
        {
            var setMethod = property.SetMethod;
            if (setMethod == null) return null;
            var parameterValue = Expression.Convert(
                Expression.Call(
                    Expression.Convert(container, typeof(IServiceContainer)),
                    _resolveDependencyMethod,
                    Expression.Constant(property.PropertyType),
                    Expression.Constant(null, typeof(string))),
                property.PropertyType); // container.GetInstance($PropertyType$, null)
            return Expression.Call(setMethod.DeclaringType != null ? (Expression)Expression.Convert(instance, setMethod.DeclaringType) : instance,
                setMethod, parameterValue);// instance.$property$ = value
        }

        public static Action<IServiceContainer, object> GetOrCreate(Type typeToBuild, bool includeProperties, bool includeMethods)
        {
            return _cache.GetOrAdd(new DynamicInjectionKey(typeToBuild, includeProperties, includeMethods), key =>
            {
                var containerParameter = Expression.Parameter(typeof(IServiceContainer), "container");
                var instanceParameter = Expression.Parameter(typeToBuild, "instance");
                var buildPlanExpressions = new Queue<Expression>();
                if (key.IncludeProperties)
                {
                    foreach (var property in InjectionAttribute.GetProperties(key.InjectType))
                    {
                        var expr = GetPropertyExpression(containerParameter, instanceParameter, property);
                        if (expr != null)
                        {
                            buildPlanExpressions.Enqueue(expr);
                        }
                    }
                }
                if (key.IncludeMethods)
                {
                    foreach (var method in InjectionAttribute.GetMethods(key.InjectType))
                    {
                        buildPlanExpressions.Enqueue(Expression.Call(
                            method.DeclaringType != null ? (Expression)Expression.Convert(instanceParameter, method.DeclaringType) : instanceParameter,
                            method, GetParameterExpressions(containerParameter, method)));
                    }
                }
                var planDelegate = buildPlanExpressions.Count > 0 ? Expression.Lambda(Expression.Block(buildPlanExpressions), containerParameter, instanceParameter).Compile() : null;
                return (container, instance) =>
                {
                    try
                    {
                        planDelegate?.DynamicInvoke(container, instance);
                    }
                    catch (TargetInvocationException e)
                    {
                        throw e.InnerException;
                    }
                };
            });
        }
    }
}
