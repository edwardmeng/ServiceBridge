using System;
using System.Collections.Generic;
using System.Linq;

namespace ServiceBridge.Unity.AspNet
{
    internal class PerRequestInstanceContainer : IDisposable
    {
        private Dictionary<Guid, object> _values = new Dictionary<Guid, object>();

        public void Dispose()
        {
            if (_values != null)
            {
                foreach (var instance in _values.Values.OfType<IDisposable>())
                {
                    instance?.Dispose();
                }
                _values = null;
            }
        }

        public object GetValue(Guid key)
        {
            if (_values == null) return null;
            object result;
            _values.TryGetValue(key, out result);
            return result;
        }

        public void SetValue(Guid key, object value)
        {
            if (_values == null) return;
            _values[key] = value;
        }

        public void Remove(Guid key)
        {
            _values?.Remove(key);
        }
    }
}
