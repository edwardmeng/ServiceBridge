using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Castle.MicroKernel;

namespace Wheatech.ServiceModel.Windsor
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
            _resolveDependencyMethod = typeof(IKernel).GetMethod("Resolve", BindingFlags.Instance | BindingFlags.Public, null, new[] { typeof(Type) }, null);
        }

        public DynamicInjectionBuilder(Type typeToBuild)
        {
            _typeToBuild = typeToBuild;
            _containerParameter = Expression.Parameter(typeof(IKernel), "kernel");
            _instanceParameter = Expression.Parameter(typeToBuild, "instance");
            _buildPlanExpressions = new Queue<Expression>();
        }

        private IEnumerable<Expression> GetParameterExpressions(MethodInfo method)
        {
            foreach (var parameter in method.GetParameters())
            {
                yield return Expression.Convert(
                        Expression.Call(
                            Expression.Convert(_containerParameter, typeof(IKernel)),
                            _resolveDependencyMethod,
                            Expression.Constant(parameter.ParameterType)), parameter.ParameterType);
            }
        }

        public Action<IKernel, object> Build()
        {
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
