namespace Wheatech.ServiceModel.Autofac
{
    public enum ServiceLifetime
    {
        Singleton,
        PerDependency,
        PerLifetimeScope,
        PerRequest
    }
}
