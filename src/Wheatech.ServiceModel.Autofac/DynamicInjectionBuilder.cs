using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Autofac;

namespace Wheatech.ServiceModel.Autofac
{
    internal class DynamicInjectionBuilder
    {
        private static readonly MethodInfo _resolveDependencyMethod;
        private static readonly ConcurrentDictionary<Type, Action<IContainer, object>> _cache;

        static DynamicInjectionBuilder()
        {
            _cache = new ConcurrentDictionary<Type, Action<IContainer, object>>();
            _resolveDependencyMethod = typeof(ResolutionExtensions).GetMethod("Resolve", BindingFlags.Static | BindingFlags.Public, null, new[] { typeof(IComponentContext), typeof(Type) }, null);
        }

        private static IEnumerable<Expression> GetParameterExpressions(ParameterExpression container, MethodInfo method)
        {
            foreach (var parameter in method.GetParameters())
            {
                yield return Expression.Convert(
                        Expression.Call(_resolveDependencyMethod,
                            Expression.Convert(container, typeof(IComponentContext)),
                            Expression.Constant(parameter.ParameterType)), parameter.ParameterType);
            }
        }

        private static Expression GetPropertyExpression(ParameterExpression container, ParameterExpression instance, PropertyInfo property)
        {
            var setMethod = property.SetMethod;
            if (setMethod == null) return null;
            var parameterValue = Expression.Convert(
                Expression.Call(_resolveDependencyMethod, Expression.Convert(container, typeof(IComponentContext)),
                    Expression.Constant(property.PropertyType)), property.PropertyType);
            return Expression.Call(setMethod.DeclaringType != null ? (Expression)Expression.Convert(instance, setMethod.DeclaringType) : instance,
                setMethod, parameterValue);
        }

        public static Action<IContainer, object> Build(Type typeToBuild)
        {
            return _cache.GetOrAdd(typeToBuild, type =>
            {
                var buildPlanExpressions = new Queue<Expression>();
                var containerParameter = Expression.Parameter(typeof(IContainer), "container");
                var instanceParameter = Expression.Parameter(type, "instance");
                foreach (var property in InjectionAttribute.GetProperties(type))
                {
                    var expr = GetPropertyExpression(containerParameter, instanceParameter, property);
                    if (expr != null)
                    {
                        buildPlanExpressions.Enqueue(expr);
                    }
                }
                foreach (var method in InjectionAttribute.GetMethods(type))
                {
                    buildPlanExpressions.Enqueue(Expression.Call(
                        method.DeclaringType != null ? (Expression) Expression.Convert(instanceParameter, method.DeclaringType) : instanceParameter,
                        method, GetParameterExpressions(containerParameter, method)));
                }
                var planDelegate = buildPlanExpressions.Count > 0
                    ? Expression.Lambda(Expression.Block(buildPlanExpressions), containerParameter, instanceParameter).Compile()
                    : null;
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
