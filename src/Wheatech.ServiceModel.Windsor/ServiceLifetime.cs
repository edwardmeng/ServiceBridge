namespace Wheatech.ServiceModel.Windsor
{
    public enum ServiceLifetime
    {
        Poolable,
        Scoped,
        Singleton,
        Transient,
        PerThread,
        PerRequest
    }
}
