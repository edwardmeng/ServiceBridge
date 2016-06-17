namespace Wheatech.ServiceModel.Unity.Interception
{
    public class UnityServiceContainerExtension : IServiceContainerExtension
    {
        public void Initialize(IServiceContainer container)
        {
            ((UnityServiceContainer)container).AddUnityExtension(new UnityInterceptionExtension());
        }

        public void Remove(IServiceContainer container)
        {
            ((UnityInterceptionExtension)((UnityServiceContainer)container).GetUnityExtension(typeof(UnityInterceptionExtension))).Remove();
        }
    }
}
