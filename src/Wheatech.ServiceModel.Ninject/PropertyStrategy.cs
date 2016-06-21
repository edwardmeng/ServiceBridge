using System;
using Ninject.Components;
using Ninject.Injection;
using Ninject.Planning;
using Ninject.Planning.Strategies;
using Ninject.Selection;

namespace Wheatech.ServiceModel.Ninject
{
    /// <summary>
    /// Adds directives to plans indicating which properties should be injected during activation.
    /// </summary>
    internal class PropertyStrategy : NinjectComponent, IPlanningStrategy
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
        /// Initializes a new instance of the <see cref="PropertyStrategy"/> class.
        /// </summary>
        /// <param name="selector">The selector component.</param>
        /// <param name="injectorFactory">The injector factory component.</param>
        public PropertyStrategy(ISelector selector, IInjectorFactory injectorFactory)
        {
            Selector = selector;
            InjectorFactory = injectorFactory;
        }

        /// <summary>
        /// Adds a <see cref="PropertyDirective"/> to the plan for each property
        /// that should be injected.
        /// </summary>
        /// <param name="plan">The plan that is being generated.</param>
        public void Execute(IPlan plan)
        {
            if (plan == null) throw new ArgumentNullException(nameof(plan));
            var properties = Selector.SelectPropertiesForInjection(plan.Type);
            if (properties != null)
            {
                foreach (var property in properties)
                {
                    plan.Add(new PropertyDirective(property, InjectorFactory.Create(property)));
                }
            }
        }
    }
}
