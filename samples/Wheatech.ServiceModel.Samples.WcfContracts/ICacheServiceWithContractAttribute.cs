using System.ServiceModel;
using Wheatech.ServiceModel.Wcf;

namespace Wheatech.ServiceModel.Samples.WcfContracts
{
    [ContainerService]
    [ServiceContract(ConfigurationName = "ICacheServiceWithContractAttribute")]
    public interface ICacheServiceWithContractAttribute
    {
        [OperationContract]
        string GetVale(string key);

        [OperationContract]
        void SetVale(string key, object value);
    }
}
