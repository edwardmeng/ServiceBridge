using System;

namespace ServiceBridge.DynamicInjection
{
    internal class DynamicInjectionKey
    {
        public DynamicInjectionKey(Type injectType, bool includeProperties, bool includeMethods)
        {
            InjectType = injectType;
            IncludeProperties = includeProperties;
            IncludeMethods = includeMethods;
        }

        public Type InjectType { get; }

        public bool IncludeMethods { get; }

        public bool IncludeProperties { get; }

        protected bool Equals(DynamicInjectionKey other)
        {
            return InjectType == other.InjectType && IncludeProperties == other.IncludeProperties && IncludeMethods == other.IncludeMethods;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((DynamicInjectionKey) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = InjectType.GetHashCode();
                hashCode = (hashCode*397) ^ IncludeProperties.GetHashCode();
                hashCode = (hashCode*397) ^ IncludeMethods.GetHashCode();
                return hashCode;
            }
        }
    }
}