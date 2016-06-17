namespace Wheatech.ServiceModel
{
    public interface IServiceContainerExtension
    {
        void Initialize(IServiceContainer container);

        void Remove(IServiceContainer container);
    }
}
