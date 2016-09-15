using System;

namespace ServiceBridge.ServiceModel
{
    internal class ClientServiceKey
    {
        public ClientServiceKey(Type contractType, string configurationName)
        {
            ContractType = contractType;
            ConfigurationName = configurationName;
        }

        public Type ContractType { get; }

        public string ConfigurationName { get; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            var other = obj as ClientServiceKey;
            if (other == null) return false;
            return ContractType == other.ContractType && ConfigurationName == other.ConfigurationName;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (ContractType.GetHashCode()*397) ^ (ConfigurationName?.GetHashCode() ?? 0);
            }
        }
    }
}