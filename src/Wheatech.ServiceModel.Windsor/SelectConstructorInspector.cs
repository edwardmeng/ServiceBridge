using System;
using System.Linq;
using System.Reflection;
using Castle.Core;
using Castle.MicroKernel;
using Castle.MicroKernel.ModelBuilder;

namespace Wheatech.ServiceModel.Windsor
{
    /// <summary>
    ///   This implementation of <see cref = "IContributeComponentModelConstruction" />
    ///   collects all available constructors and populates them in the model
    ///   as candidates. The Kernel will pick up one of the candidates
    ///   according to a heuristic.
    /// </summary>
	[Serializable]
    internal class SelectConstructorInspector : IContributeComponentModelConstruction
    {
        public virtual void ProcessModel(IKernel kernel, ComponentModel model)
        {
            var targetType = model.Implementation;
            var constructors = InjectionAttribute.GetConstructors(targetType).Where(IsVisibleToContainer).ToArray();
            if (constructors.Length == 0)
            {
                constructors = targetType.GetConstructors(BindingFlags.Public | BindingFlags.Instance)
                    .Where(IsVisibleToContainer).ToArray();
            }

            foreach (var constructor in constructors)
            {
                // We register each public constructor
                // and let the ComponentFactory select an 
                // eligible amongst the candidates later
                model.AddConstructor(CreateConstructorCandidate(model, constructor));
            }
        }

        protected virtual ConstructorCandidate CreateConstructorCandidate(ComponentModel model, ConstructorInfo constructor)
        {
            var parameters = constructor.GetParameters();
            var dependencies = Array.ConvertAll(parameters, BuildParameterDependency);
            return new ConstructorCandidate(constructor, dependencies);
        }

        private static ConstructorDependencyModel BuildParameterDependency(ParameterInfo parameter)
        {
            return new ConstructorDependencyModel(parameter);
        }

        protected virtual bool IsVisibleToContainer(ConstructorInfo constructor)
        {
            return !constructor.IsDefined(typeof(DoNotSelectAttribute));
        }
    }
}
