using System;
using System.Collections.Generic;
using System.Web;
using Microsoft.Practices.Unity;

namespace Wheatech.ServiceModel.Unity
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
            var values = EnsureValues();
            if (values == null) return null;
            object result;
            values.TryGetValue(_key, out result);
            return result;
        }

        /// <summary>
        /// Stores the given value into backing store for retrieval later when requested
        /// in the current web request.
        /// </summary>
        /// <param name="newValue">The object being stored.</param>
        public override void SetValue(object newValue)
        {
            var values = EnsureValues();
            if (values == null) return;
            values[_key] = newValue;
        }

        /// <summary>
        /// Remove the given object from backing store.
        /// </summary>
        public override void RemoveValue()
        {
            var values = EnsureValues();
            values?.Remove(_key);
        }

        private static Dictionary<Guid, object> EnsureValues()
        {
            if (HttpContext.Current == null) return null;
            var values = (Dictionary<Guid, object>) HttpContext.Current.Items[typeof(PerRequestLifetimeManager)];
            // no need for locking, values is TLS
            if (values == null)
            {
                HttpContext.Current.Items[typeof(PerRequestLifetimeManager)] = values = new Dictionary<Guid, object>();
            }
            return values;
        }
    }
}
