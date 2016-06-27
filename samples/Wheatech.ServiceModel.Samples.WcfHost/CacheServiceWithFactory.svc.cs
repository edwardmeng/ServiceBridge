﻿using System.ServiceModel;
using Wheatech.ServiceModel.Sample.Components;
using Wheatech.ServiceModel.Samples.WcfContracts;

namespace Wheatech.ServiceModel.Samples.WcfHost
{
    [ServiceBehavior(ConfigurationName = "CacheServiceWithFactory")]
    public class CacheServiceWithFactory : ICacheServiceWithFactory
    {
        [Injection]
        public ICacheRepository Repository { get; set; }

        public string GetVale(string key)
        {
            return (string)Repository.GetVale(key);
        }

        public void SetVale(string key, string value)
        {
            Repository.SetVale(key, value);
        }
    }
}