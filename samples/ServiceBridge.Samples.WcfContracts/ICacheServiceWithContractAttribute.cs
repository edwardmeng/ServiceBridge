using System.ServiceModel;
using ServiceBridge.ServiceModel;

namespace ServiceBridge.Samples.WcfContracts
{
    [ContainerService]
    [ServiceContract(ConfigurationName = "ICacheServiceWithContractAttribute")]
    public interface ICacheServiceWithContractAttribute
    {
        [OperationContract]
        string GetVale(string key);

        [OperationContract]
        void SetVale(string key, string value);
    }
}
