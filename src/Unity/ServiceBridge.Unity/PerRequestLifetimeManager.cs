using System;
using System.Web;
using Microsoft.Practices.Unity;

namespace ServiceBridge.Unity
{
    /// <summary>
    /// A <see cref="LifetimeManager"/> that holds the instances given to it, 
    /// keeping one instance per web request.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This LifetimeManager does not dispose the instances it holds.
    /// </para>
    /// </remarks>
    internal class PerRequestLifetimeManager : LifetimeManager
    {
        private readonly Guid _key;

        /// <summary>
        /// Initializes a new instance of the <see cref="PerRequestLifetimeManager"/> class.
        /// </summary>
        public PerRequestLifetimeManager()
        {
            _key = Guid.NewGuid();
        }

        /// <summary>
        /// Retrieve a value from the backing store associated with this Lifetime policy for the 
        /// current web request.
        /// </summary>
        /// <returns>the object desired, or <see langword="null"/> if no such object is currently 
        /// stored for the current web request.</returns>
        public override object GetValue()
        {
            return EnsureValues()?.GetValue(_key);
        }

        /// <summary>
        /// Stores the given value into backing store for retrieval later when requested
        /// in the current web request.
        /// </summary>
        /// <param name="newValue">The object being stored.</param>
        public override void SetValue(object newValue)
        {
            EnsureValues()?.SetValue(_key,newValue);
        }

        /// <summary>
        /// Remove the given object from backing store.
        /// </summary>
        public override void RemoveValue()
        {
            var values = EnsureValues();
            values?.Remove(_key);
        }

        private static PerRequestInstanceContainer EnsureValues()
        {
            var context = ServiceContainer.HostContext as HttpContext;
            if (context == null) return null;
            if (!context.Items.Contains(typeof(PerRequestLifetimeManager)))
            {
                lock (context.Items.SyncRoot)
                {
                    if (!context.Items.Contains(typeof(PerRequestLifetimeManager)))
                    {
                        var values = new PerRequestInstanceContainer();
                        context.Items[typeof(PerRequestLifetimeManager)] = values;
                        context.DisposeOnPipelineCompleted(values);
                        return values;
                    }
                }
            }
            return (PerRequestInstanceContainer)context.Items[typeof(PerRequestLifetimeManager)];
        }
    }
}
