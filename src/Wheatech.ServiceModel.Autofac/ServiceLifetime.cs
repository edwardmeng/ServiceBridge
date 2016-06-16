namespace Wheatech.ServiceModel.Autofac
{
    public enum ServiceLifetime
    {
        SingleInstance,
        InstancePerDependency,
        InstancePerLifetimeScope,
        ExternallyOwned,
        OwnedByLifetimeScope
    }
}
