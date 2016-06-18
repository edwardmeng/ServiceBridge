namespace Wheatech.ServiceModel.Ninject
{
    public enum ServiceLifetime
    {
        Transient,
        /// <summary>Gets the callback for singleton scope.</summary>
        Singleton,
        /// <summary>Gets the callback for thread scope.</summary>
        Thread
    }
}
