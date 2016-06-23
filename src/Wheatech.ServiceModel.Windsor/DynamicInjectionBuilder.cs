using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Castle.MicroKernel;

namespace Wheatech.ServiceModel.Windsor
{
    internal class DynamicInjectionBuilder
    {
        private static readonly MethodInfo _resolveDependencyMethod;
        private static readonly ConcurrentDictionary<Tuple<Type, bool, bool>, Action<IKernel, object>> _cache;

        static DynamicInjectionBuilder()
        {
            _resolveDependencyMethod = typeof(IKernel).GetMethod("Resolve", BindingFlags.Instance | BindingFlags.Public, null, new[] { typeof(Type) }, null);
            _cache = new ConcurrentDictionary<Tuple<Type, bool, bool>, Action<IKernel, object>>();
        }

        private static IEnumerable<Expression> GetParameterExpressions(ParameterExpression container, MethodInfo method)
        {
            foreach (var parameter in method.GetParameters())
            {
                yield return Expression.Convert(
                        Expression.Call(
                            Expression.Convert(container, typeof(IKernel)),
                            _resolveDependencyMethod,
                            Expression.Constant(parameter.ParameterType)), parameter.ParameterType);
            }
        }

        private static Expression GetPropertyExpression(ParameterExpression container, ParameterExpression instance, PropertyInfo property)
        {
            var setMethod = property.SetMethod;
            if (setMethod == null) return null;
            var parameterValue = Expression.Convert(
                Expression.Call(Expression.Convert(container, typeof(IKernel)), _resolveDependencyMethod, Expression.Constant(property.PropertyType)), property.PropertyType);
            return Expression.Call(setMethod.DeclaringType != null ? (Expression)Expression.Convert(instance, setMethod.DeclaringType) : instance,
                setMethod, parameterValue);
        }

        public static Action<IKernel, object> GetOrCreate(Type typeToBuild, bool includeProperties, bool includeMethods)
        {
            return _cache.GetOrAdd(Tuple.Create(typeToBuild, includeProperties, includeMethods), key =>
            {
                var containerParameter = Expression.Parameter(typeof(IKernel), "kernel");
                var instanceParameter = Expression.Parameter(typeToBuild, "instance");
                var buildPlanExpressions = new Queue<Expression>();
                if (key.Item2)
                {
                    foreach (var property in InjectionAttribute.GetProperties(key.Item1))
                    {
                        var expr = GetPropertyExpression(containerParameter, instanceParameter, property);
                        if (expr != null)
                        {
                            buildPlanExpressions.Enqueue(expr);
                        }
                    }
                }
                if (key.Item3)
                {
                    foreach (var method in InjectionAttribute.GetMethods(key.Item1))
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
