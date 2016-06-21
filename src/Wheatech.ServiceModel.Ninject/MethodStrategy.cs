using System;
using Ninject.Components;
using Ninject.Injection;
using Ninject.Planning;
using Ninject.Planning.Strategies;
using Ninject.Selection;

namespace Wheatech.ServiceModel.Ninject
{
    /// <summary>
    /// Adds directives to plans indicating which methods should be injected during activation.
    /// </summary>
    internal class MethodStrategy : NinjectComponent, IPlanningStrategy
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
        /// Initializes a new instance of the <see cref="MethodStrategy"/> class.
        /// </summary>
        /// <param name="selector">The selector component.</param>
        /// <param name="injectorFactory">The injector factory component.</param>
        public MethodStrategy(ISelector selector, IInjectorFactory injectorFactory)
        {
            Selector = selector;
            InjectorFactory = injectorFactory;
        }

        /// <summary>
        /// Adds a <see cref="MethodDirective"/> to the plan for each method
        /// that should be injected.
        /// </summary>
        /// <param name="plan">The plan that is being generated.</param>
        public void Execute(IPlan plan)
        {
            if (plan == null) throw new ArgumentNullException(nameof(plan));
            var methods = Selector.SelectMethodsForInjection(plan.Type);
            if (methods != null)
            {
                foreach (var method in methods)
                {
                    plan.Add(new MethodDirective(method, InjectorFactory.Create(method)));
                }
            }
        }
    }
}
