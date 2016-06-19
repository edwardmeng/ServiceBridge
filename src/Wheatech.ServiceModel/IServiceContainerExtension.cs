namespace Wheatech.ServiceModel
{
    /// <summary>
    /// Interface for all <see cref="IServiceContainer"/> extension objects. 
    /// </summary>
    public interface IServiceContainerExtension
    {
        /// <summary>
        /// Initial the container with this extension's functionality. 
        /// </summary>
        /// <param name="container">The container this extension to extend.</param>
        void Initialize(IServiceContainer container);

        /// <summary>
        /// Removes the extension's functions from the container. 
        /// </summary>
        /// <param name="container">The container this extension to extend.</param>
        void Remove(IServiceContainer container);
    }
}
