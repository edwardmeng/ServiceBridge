using System.Collections.Generic;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace Wheatech.ServiceModel.Unity.Interception
{
    /// <summary>
    /// A unity extension to enable the interception mechanism.
    /// </summary>
    internal class UnityInterceptionExtension : UnityContainerExtension
    {
        /// <summary>
        /// Initial the container with this extension's functionality.
        /// </summary>
        protected override void Initialize()
        {
            Context.Registering += OnRegistering;
        }

        public override void Remove()
        {
            Context.Registering -= OnRegistering;
        }

        private void OnRegistering(object sender, RegisterEventArgs e)
        {
            var interceptionBehaviorsPolicy = new InterceptionBehaviorsPolicy();
            var behaviors = (IList<NamedTypeBuildKey>)interceptionBehaviorsPolicy.BehaviorKeys;
            behaviors.Add(NamedTypeBuildKey.Make<PolicyInjectionBehavior>());
            behaviors.Add(NamedTypeBuildKey.Make<UnityInjectionBehavior>());
            Context.Policies.Set<IInterceptionBehaviorsPolicy>(interceptionBehaviorsPolicy, new NamedTypeBuildKey(e.TypeFrom, e.Name));
        }
    }
}
