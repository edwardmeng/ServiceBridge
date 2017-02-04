using Ninject.Extensions.Interception;
using Ninject.Extensions.Interception.ProxyFactory;

namespace ServiceBridge.Ninject.Interception
{
    /// <summary>
    /// Extends the functionality of the kernel, providing a proxy factory that uses Castle DynamicProxy2
    /// to generate dynamic proxies.
    /// </summary>
    public class NinjectInterceptionModule : InterceptionModule
    {
        /// <summary>
        /// Loads the module into the kernel.
        /// </summary>
        public override void Load()
        {
            Kernel?.Components.Add<IProxyFactory, NinjectDynamicProxyFactory>();
            base.Load();
        }
    }
}
