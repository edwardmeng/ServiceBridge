using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using StructureMap;

namespace Wheatech.ServiceModel.StructureMap
{
    internal static class DynamicInjectionBuilder
    {
        private static readonly MethodInfo _resolveContextDependencyMethod;
        private static readonly MethodInfo _resolveContainerDependencyMethod;
        private static readonly ConcurrentDictionary<Type, Action<IContext, object>> _registerCache;
        private static readonly ConcurrentDictionary<Type, Action<IContainer, object>> _injectCache;

        static DynamicInjectionBuilder()
        {
            _resolveContextDependencyMethod = typeof(IContext).GetMethod("GetInstance", BindingFlags.Instance | BindingFlags.Public, null, new[] { typeof(Type) }, null);
            _resolveContainerDependencyMethod = typeof(IContainer).GetMethod("GetInstance", BindingFlags.Instance | BindingFlags.Public, null, new[] { typeof(Type) }, null);
            _registerCache = new ConcurrentDictionary<Type, Action<IContext, object>>();
            _injectCache = new ConcurrentDictionary<Type, Action<IContainer, object>>();
        }

        private static IEnumerable<Expression> GetRegisterParameterExpressions(ParameterExpression context, MethodInfo method)
        {
            foreach (var parameter in method.GetParameters())
            {
                yield return Expression.Convert(
                    Expression.Call(Expression.Convert(context, typeof(IContext)),
                        _resolveContextDependencyMethod,
                        Expression.Constant(parameter.ParameterType)), parameter.ParameterType);
            }
        }

        private static IEnumerable<Expression> GetInjectParameterExpressions(ParameterExpression container, MethodInfo method)
        {
            foreach (var parameter in method.GetParameters())
            {
                yield return Expression.Convert(
                    Expression.Call(Expression.Convert(container, typeof(IContainer)),
                        _resolveContainerDependencyMethod,
                        Expression.Constant(parameter.ParameterType)), parameter.ParameterType);
            }
        }

        private static Expression GetPropertyExpression(ParameterExpression container, ParameterExpression instance, PropertyInfo property)
        {
            var setMethod = property.SetMethod;
            if (setMethod == null) return null;
            var parameterValue = Expression.Convert(
                Expression.Call(Expression.Convert(container, typeof(IContainer)), _resolveContainerDependencyMethod, Expression.Constant(property.PropertyType)), property.PropertyType);
            return Expression.Call(setMethod.DeclaringType != null ? (Expression)Expression.Convert(instance, setMethod.DeclaringType) : instance,
                setMethod, parameterValue);
        }

        public static Action<IContainer, object> GetOrCreateForInject(Type typeToBuild)
        {
            return _injectCache.GetOrAdd(typeToBuild, key =>
            {
                var containerParameter = Expression.Parameter(typeof(IContainer), "container");
                var instanceParameter = Expression.Parameter(key, "instance");
                var buildPlanExpressions = new Queue<Expression>();
                foreach (var property in InjectionAttribute.GetProperties(key))
                {
                    var expr = GetPropertyExpression(containerParameter, instanceParameter, property);
                    if (expr != null)
                    {
                        buildPlanExpressions.Enqueue(expr);
                    }
                }
                foreach (var method in InjectionAttribute.GetMethods(key))
                {
                    buildPlanExpressions.Enqueue(Expression.Call(
                        method.DeclaringType != null ? (Expression)Expression.Convert(instanceParameter, method.DeclaringType) : instanceParameter,
                        method, GetInjectParameterExpressions(containerParameter, method)));
                }
                var planDelegate = buildPlanExpressions.Count > 0 ? Expression.Lambda(Expression.Block(buildPlanExpressions), containerParameter, instanceParameter).Compile() : null;
                return (context, instance) =>
                {
                    try
                    {
                        planDelegate?.DynamicInvoke(context, instance);
                    }
                    catch (TargetInvocationException e)
                    {
                        throw e.InnerException;
                    }
                };
            });
        }

        public static Action<IContext, object> GetOrCreateForRegister(Type typeToBuild)
        {
            return _registerCache.GetOrAdd(typeToBuild, key =>
            {
                var contextParameter = Expression.Parameter(typeof(IContext), "context");
                var instanceParameter = Expression.Parameter(key, "instance");
                var buildPlanExpressions = new Queue<Expression>();
                foreach (var method in InjectionAttribute.GetMethods(key))
                {
                    buildPlanExpressions.Enqueue(Expression.Call(
                        method.DeclaringType != null ? (Expression)Expression.Convert(instanceParameter, method.DeclaringType) : instanceParameter,
                        method, GetRegisterParameterExpressions(contextParameter, method)));
                }
                var planDelegate = buildPlanExpressions.Count > 0 ? Expression.Lambda(Expression.Block(buildPlanExpressions), contextParameter, instanceParameter).Compile() : null;
                return (context, instance) =>
                {
                    try
                    {
                        planDelegate?.DynamicInvoke(context, instance);
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
