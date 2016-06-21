using System;
using Ninject.Components;
using Ninject.Injection;
using Ninject.Planning;
using Ninject.Planning.Strategies;
using Ninject.Selection;

namespace Wheatech.ServiceModel.Ninject
{
    /// <summary>
    /// Adds a directive to plans indicating which constructor should be injected during activation.
    /// </summary>
    internal class ConstructorStrategy : NinjectComponent, IPlanningStrategy
    {
        /// <summary>
        /// Gets the selector component.
        /// </summary>
        public ISelector Selector { get; }

        /// <summary>
        /// Gets the injector factory component.
        /// </summary>
        public IInjectorFactory InjectorFactory { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstructorStrategy"/> class.
        /// </summary>
        /// <param name="selector">The selector component.</param>
        /// <param name="injectorFactory">The injector factory component.</param>
        public ConstructorStrategy(ISelector selector, IInjectorFactory injectorFactory)
        {
            Selector = selector;
            InjectorFactory = injectorFactory;
        }

        /// <summary>
        /// Adds a <see cref="ConstructorDirective"/> to the plan for the constructor
        /// that should be injected.
        /// </summary>
        /// <param name="plan">The plan that is being generated.</param>
        public void Execute(IPlan plan)
        {
            if (plan == null) throw new ArgumentNullException(nameof(plan));
            var constructors = Selector.SelectConstructorsForInjection(plan.Type);
            if (constructors != null)
            {
                foreach (var constructor in constructors)
                {
                    plan.Add(new ConstructorDirective(constructor, InjectorFactory.Create(constructor)));
                }
            }
        }
    }
}
