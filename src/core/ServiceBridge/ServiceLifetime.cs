namespace ServiceBridge
{
    /// <summary>
    /// Enumeration used to mark the instance's lifetime.
    /// </summary>
    public enum ServiceLifetime
    {
        /// <summary>
        /// Instances are instantiated once, and shared between all clients.
        /// </summary>
        Singleton,
        /// <summary>
        /// Instances are created new every time.
        /// </summary>
        Transient,
        /// <summary>
        /// Keeping one instance per thread.
        /// </summary>
        PerThread,
        /// <summary>
        /// Keeping one instance per web request.
        /// </summary>
        PerRequest
    }
}
