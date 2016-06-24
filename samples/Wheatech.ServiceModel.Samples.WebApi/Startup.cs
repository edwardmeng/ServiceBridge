using Wheatech.Activation;
using Wheatech.ServiceModel.Autofac;
using Wheatech.ServiceModel.Autofac.Interception;

namespace Wheatech.ServiceModel.Samples.WebApi
{
    public class Startup
    {
        public Startup(IActivatingEnvironment environment)
        {
            environment.UseAutofac().EnableInterception();
        }
    }
}