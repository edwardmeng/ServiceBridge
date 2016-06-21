using System;
using System.Linq.Expressions;
using System.Reflection;
using Castle.DynamicProxy;
using StructureMap;
using StructureMap.Building.Interception;
using StructureMap.TypeRules;
using Wheatech.ServiceModel.DynamicProxy;
using Wheatech.ServiceModel.Interception;
using IInterceptor = StructureMap.Building.Interception.IInterceptor;

namespace Wheatech.ServiceModel.StructureMap.Interception
{
    internal class DynamicProxyInterceptor : IInterceptor
    {

        private readonly string _description;
        private readonly Type _serviceType;
        private readonly Type _typeToProxy;
        private static readonly MethodInfo CreateProxyMethod;

        static DynamicProxyInterceptor()
        {
            CreateProxyMethod = typeof(DynamicProxyInterceptor).GetMethod("CreateProxy", BindingFlags.Static | BindingFlags.Public);
        }

        public DynamicProxyInterceptor(Type serviceType, Type typeToProxy, string description = null)
        {
            _serviceType = serviceType;
            _typeToProxy = typeToProxy;
            _description = description;
        }

        public virtual string Description => _description ?? $"DynamicProxyInterceptor of {_typeToProxy.GetFullName()}";

        public Expression ToExpression(Policies policies, ParameterExpression session, ParameterExpression variable)
        {
            return Expression.Call(CreateProxyMethod.MakeGenericMethod(_serviceType), session, Expression.Constant(_typeToProxy), Expression.Convert(variable, _serviceType));
        }

        public static T CreateProxy<T>(IContext context, Type typeToProxy, T instance)
        {
            return (T)new ProxyGenerator().CreateClassProxyWithTarget(typeToProxy, instance,
                new ServiceInterceptor((PipelineManager) context.GetInstance(typeof(PipelineManager)), ServiceContainer.Current));
        }

        public InterceptorRole Role => InterceptorRole.Decorates;

        public Type Accepts => _serviceType;

        public Type Returns => _serviceType;

        protected bool Equals(DynamicProxyInterceptor other)
        {
            return other != null && _serviceType == other._serviceType && _typeToProxy == other._typeToProxy;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((DynamicProxyInterceptor)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (_serviceType.GetHashCode() * 397) ^ _typeToProxy.GetHashCode();
            }
        }

        public override string ToString()
        {
            return $"Interceptor of {_typeToProxy.GetFullName()}: {Description}";
        }
    }
}
