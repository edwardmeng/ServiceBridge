using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Autofac;

namespace Wheatech.ServiceModel.Autofac
{
    internal class DynamicInjectionBuilder
    {
        private readonly Type _typeToBuild;
        private readonly ParameterExpression _containerParameter;
        private readonly ParameterExpression _instanceParameter;
        private readonly Queue<Expression> _buildPlanExpressions;
        private static readonly MethodInfo _resolveDependencyMethod;

        static DynamicInjectionBuilder()
        {
            _resolveDependencyMethod = typeof(ResolutionExtensions).GetMethod("Resolve", BindingFlags.Static | BindingFlags.Public, null, new[] { typeof(IComponentContext), typeof(Type) }, null);
        }

        public DynamicInjectionBuilder(Type typeToBuild)
        {
            _typeToBuild = typeToBuild;
            _containerParameter = Expression.Parameter(typeof(IContainer), "container");
            _instanceParameter = Expression.Parameter(typeToBuild, "instance");
            _buildPlanExpressions = new Queue<Expression>();
        }

        private IEnumerable<Expression> GetParameterExpressions(MethodInfo method)
        {
            foreach (var parameter in method.GetParameters())
            {
                yield return Expression.Convert(
                        Expression.Call(_resolveDependencyMethod,
                            Expression.Convert(_containerParameter, typeof(IComponentContext)),
                            Expression.Constant(parameter.ParameterType)), parameter.ParameterType);
            }
        }

        private Expression GetPropertyExpression(PropertyInfo property)
        {
            var setMethod = property.SetMethod;
            if (setMethod == null) return null;
            var parameterValue = Expression.Convert(
                Expression.Call(_resolveDependencyMethod, Expression.Convert(_containerParameter, typeof(IComponentContext)),
                    Expression.Constant(property.PropertyType)), property.PropertyType);
            return Expression.Call(setMethod.DeclaringType != null ? (Expression)Expression.Convert(_instanceParameter, setMethod.DeclaringType) : _instanceParameter,
                setMethod, parameterValue);
        }

        public Action<IContainer, object> Build()
        {
            foreach (var property in InjectionAttribute.GetProperties(_typeToBuild))
            {
                var expr = GetPropertyExpression(property);
                if (expr != null)
                {
                    _buildPlanExpressions.Enqueue(expr);
                }
            }
            foreach (var method in InjectionAttribute.GetMethods(_typeToBuild))
            {
                _buildPlanExpressions.Enqueue(Expression.Call(
                    method.DeclaringType != null ? (Expression)Expression.Convert(_instanceParameter, method.DeclaringType) : _instanceParameter,
                    method, GetParameterExpressions(method)));
            }
            var planDelegate = _buildPlanExpressions.Count > 0 ? Expression.Lambda(Expression.Block(_buildPlanExpressions), _containerParameter, _instanceParameter).Compile() : null;
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
        }
    }
}
