using System;
using MassActivation;
using ServiceBridge.Samples.WcfContracts;

namespace ServiceBridge.Samples.WcfClient
{
    class Program
    {
        static void Main()
        {
            ApplicationActivator.UseEnvironment(EnvironmentName.Development).Startup();

            try
            {
                Console.WriteLine("Accessing the ICacheServiceWithBehavior...");
                var service1 = ServiceContainer.GetInstance<ICacheServiceWithBehavior>();
                Console.WriteLine("ICacheServiceWithBehavior returns value: " + service1.GetVale("Sample"));

                Console.WriteLine("Accessing the ICacheServiceWithContractAttribute...");
                var service2 = ServiceContainer.GetInstance<ICacheServiceWithContractAttribute>();
                Console.WriteLine("ICacheServiceWithContractAttribute returns value: " + service2.GetVale("Sample"));

                Console.WriteLine("Accessing the ICacheServiceWithFactory...");
                var service3 = ServiceContainer.GetInstance<ICacheServiceWithFactory>();
                Console.WriteLine("ICacheServiceWithFactory returns value: " + service3.GetVale("Sample"));

                Console.WriteLine("Accessing the ICacheServiceWithServiceAttribute...");
                var service4 = ServiceContainer.GetInstance<ICacheServiceWithServiceAttribute>();
                Console.WriteLine("ICacheServiceWithServiceAttribute returns value: " + service4.GetVale("Sample"));

                Console.WriteLine("Success");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.GetType().FullName);
                Console.Error.WriteLine(ex.Message);
            }

            Console.ReadLine();
        }
    }
}
