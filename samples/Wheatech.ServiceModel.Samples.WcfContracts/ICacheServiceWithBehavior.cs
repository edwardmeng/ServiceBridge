using System.ServiceModel;

namespace Wheatech.ServiceModel.Samples.WcfContracts
{
    [ServiceContract(ConfigurationName = "ICacheServiceWithBehavior")]
    public interface ICacheServiceWithBehavior
    {
        [OperationContract]
        string GetVale(string key);

        [OperationContract]
        void SetVale(string key, string value);
    }
}
