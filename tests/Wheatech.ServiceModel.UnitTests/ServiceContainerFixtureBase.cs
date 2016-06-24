using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Wheatech.ServiceModel.UnitTests.Components;
using Xunit;

namespace Wheatech.ServiceModel.UnitTests
{
    public abstract class ServiceContainerFixtureBase
    {
        protected abstract IServiceContainer CreateContainer();

        protected ServiceContainerFixtureBase()
        {
            ServiceContainer.SetProvider(CreateContainer);
            ServiceContainer.Register<ILogger, AdvancedLogger>().Register<ILogger, SimpleLogger>("Simple").Register<ILogger, AdvancedLogger>("Advanced");
            ServiceContainer.Register<ICanChangeParameters, CanChangeParametersTarget>().Register<ObjectWithInjection>();
        }

        [Fact]
        public void ResolveSeviceContainer()
        {
            Assert.Equal(ServiceContainer.Current, ServiceContainer.GetInstance<IServiceContainer>());
        }

        [Fact]
        public void GetInstance()
        {
            var instance = ServiceContainer.GetInstance<ILogger>();
            Assert.NotNull(instance);
        }

        [Fact]
        public void IsRegistered()
        {
            Assert.True(ServiceContainer.IsRegistered<ILogger>());
            Assert.True(ServiceContainer.IsRegistered<ILogger>("Simple"));
            Assert.True(ServiceContainer.IsRegistered(typeof(ILogger), "Advanced"));
            Assert.False(ServiceContainer.IsRegistered<IDictionary>());
        }

        [Fact]
        public void UnregisteredInterfaceShouldBeNull()
        {
            Assert.Null(ServiceContainer.GetInstance<IDictionary>());
        }

        [Fact]
        public void UnregisteredObjectShouldNotBeNull()
        {
            Assert.True(ServiceContainer.IsRegistered<ILogger>());
            Assert.NotNull(ServiceContainer.GetInstance<UnregisteredInjectionObject>());
            Assert.False(ServiceContainer.IsRegistered<UnregisteredInjectionObject>());
            Assert.NotNull(ServiceContainer.GetInstance<UnregisteredInjectionObject>());
        }

        [Fact]
        public void GetNamedInstance()
        {
            var instance = ServiceContainer.GetInstance<ILogger>("Advanced");
            Assert.IsAssignableFrom<AdvancedLogger>(instance);
        }

        [Fact]
        public void GetNamedInstance2()
        {
            ILogger instance = ServiceContainer.GetInstance<ILogger>("Simple");
            Assert.IsAssignableFrom<SimpleLogger>(instance);
        }

        [Fact]
        public void GetUnknownInstance2()
        {
            Assert.Throws<ActivationException>(() => ServiceContainer.GetInstance<ILogger>("test"));
        }

        [Fact]
        public void GetAllInstances()
        {
            var instances = ServiceContainer.GetAllInstances<ILogger>();
            IList<ILogger> list = new List<ILogger>(instances);
            Assert.Equal(3, list.Count);
        }

        [Fact]
        public void GetAllInstance_ForUnknownType_ReturnEmptyEnumerable()
        {
            IEnumerable<IDictionary> instances = ServiceContainer.GetAllInstances<IDictionary>();
            IList<IDictionary> list = new List<IDictionary>(instances);
            Assert.Equal(0, list.Count);
        }

        [Fact]
        public void GenericOverload_GetInstance()
        {
            Assert.Equal(
                ServiceContainer.GetInstance<ILogger>().GetType(),
                ServiceContainer.GetInstance(typeof(ILogger), null).GetType());
        }

        [Fact]
        public void GenericOverload_GetInstance_WithName()
        {
            Assert.Equal(
                ServiceContainer.GetInstance<ILogger>("Advanced").GetType(),
                ServiceContainer.GetInstance(typeof(ILogger), "Advanced").GetType());
        }

        [Fact]
        public void Overload_GetInstance_NoName_And_NullName()
        {
            Assert.Equal(
                ServiceContainer.GetInstance<ILogger>().GetType(),
                ServiceContainer.GetInstance<ILogger>(null).GetType());
        }

        [Fact]
        public void GenericOverload_GetAllInstances()
        {
            List<ILogger> genericLoggers = new List<ILogger>(ServiceContainer.GetAllInstances<ILogger>());
            List<object> plainLoggers = new List<object>(ServiceContainer.GetAllInstances(typeof(ILogger)));
            Assert.Equal(genericLoggers.Count, plainLoggers.Count);
            for (int i = 0; i < genericLoggers.Count; i++)
            {
                Assert.Equal(
                    genericLoggers[i].GetType(),
                    plainLoggers[i].GetType());
            }
        }

        #region Injection

        [Fact]
        public void RegistrationInjectConstructor()
        {
            var o = ServiceContainer.GetInstance<ObjectWithInjection>();

            Assert.NotNull(o.InjectionFromConstructor);
            Assert.Null(o.NotInjectionFromConstructor);
        }

        [Fact]
        public void RegistrationInjectMethod()
        {
            var o = ServiceContainer.GetInstance<ObjectWithInjection>();
            Assert.NotNull(o.InjectionFromMethod);
            Assert.Null(o.NotInjection);
        }

        [Fact]
        public void RegistrationInjectProperty()
        {
            var o = ServiceContainer.GetInstance<ObjectWithInjection>();

            Assert.NotNull(o.InjectionFromProperty);
            Assert.Null(o.NotInjection);
        }

        [Fact]
        public void InjectRegisteredInstanceMethod()
        {
            var o = new ObjectWithInjection();
            ServiceContainer.InjectInstance(o);
            Assert.NotNull(o.InjectionFromMethod);
            Assert.Null(o.NotInjection);
            Assert.Null(o.InjectionFromConstructor);
        }

        [Fact]
        public void InjectRegisteredInstanceProperty()
        {
            var o = new ObjectWithInjection();
            ServiceContainer.InjectInstance(o);
            Assert.NotNull(o.InjectionFromProperty);
            Assert.Null(o.NotInjection);
            Assert.Null(o.InjectionFromConstructor);
        }

        [Fact]
        public void InjectUnregisteredInstanceMethod()
        {
            var o = new UnregisteredInjectionObject();
            ServiceContainer.InjectInstance(o);
            Assert.NotNull(o.InjectionFromMethod);
            Assert.Null(o.NotInjection);
        }

        [Fact]
        public void InjectUnregisteredInstanceProperty()
        {
            var o = new UnregisteredInjectionObject();
            ServiceContainer.InjectInstance(o);
            Assert.NotNull(o.InjectionFromProperty);
            Assert.Null(o.NotInjection);
        }

        #endregion

        #region Interception

        [Fact]
        public void InterceptorsCanChangeInputsBeforeTargetIsCalled()
        {
            var intercepted = ServiceContainer.GetInstance<ICanChangeParameters>();
            intercepted.MostRecentInput = 0;
            intercepted.DoSomething(2);
            Assert.Equal(4, intercepted.MostRecentInput);
        }

        [Fact]
        public void HandlersCanChangeOutputsAfterTargetReturns()
        {
            var intercepted = ServiceContainer.GetInstance<ICanChangeParameters>();
            int output;

            intercepted.DoSomethingElse(2, out output);
            intercepted.MostRecentInput = 0;

            Assert.Equal((2 + 5) * 3, output);
        }

        [Fact]
        public void HandlersCanChangeRefsAfterTargetReturns()
        {
            var intercepted = ServiceContainer.GetInstance<ICanChangeParameters>();
            int output = 3;

            intercepted.DoSomethingElseWithRef(2, ref output);

            Assert.Equal((2 + 3 + 5) * 3, output);
        }
        #endregion

        #region Lifetime

        protected abstract string WebName { get; }

        [Fact]
        public void SingletonInThreads()
        {
            ServiceContainer.Register<LifetimeObject>(ServiceLifetime.Singleton);
            LifetimeObject result1 = null;
            LifetimeObject result2 = null;
            Thread thread1 = new Thread(delegate ()
            {
                result1 = ServiceContainer.GetInstance<LifetimeObject>();
            });

            Thread thread2 = new Thread(delegate ()
            {
                result2 = ServiceContainer.GetInstance<LifetimeObject>();
            });
            thread1.Start();
            thread2.Start();

            thread2.Join();
            thread1.Join();

            Assert.NotNull(result1);
            Assert.Same(result1, result2);
        }

        [Fact]
        public void SingletonInstances()
        {
            ServiceContainer.Register<LifetimeObject>(ServiceLifetime.Singleton);
            LifetimeObject result1 = ServiceContainer.GetInstance<LifetimeObject>();
            LifetimeObject result2 = ServiceContainer.GetInstance<LifetimeObject>();
            Assert.NotNull(result1);
            Assert.Same(result1, result2);
        }

        [Fact]
        public void PerThreadInstances()
        {
            ServiceContainer.Register<LifetimeObject>(ServiceLifetime.PerThread);
            LifetimeObject result1 = null;
            LifetimeObject result2 = null;
            LifetimeObject result3 = null;
            Thread thread1 = new Thread(delegate ()
            {
                result1 = ServiceContainer.GetInstance<LifetimeObject>();
            });

            Thread thread2 = new Thread(delegate ()
            {
                result2 = ServiceContainer.GetInstance<LifetimeObject>();
                result3 = ServiceContainer.GetInstance<LifetimeObject>();
            });
            thread1.Start();
            thread2.Start();

            thread2.Join();
            thread1.Join();

            Assert.NotNull(result1);
            Assert.NotNull(result2);
            Assert.NotNull(result3);
            Assert.NotSame(result1, result2);
            Assert.Same(result3, result2);
        }

        [Fact]
        public async Task WebApiPerRequestLifetime()
        {
            Assert.Equal("Success", JsonConvert.DeserializeObject(await ProcessWebRequest($"http://localhost:62232/api/Lifetime?container={WebName}")));
            Assert.Equal("12345", JsonConvert.DeserializeObject(await ProcessWebRequest("http://localhost:62232/api/Lifetime?value=12345")));
            Assert.Equal("Default", JsonConvert.DeserializeObject(await ProcessWebRequest("http://localhost:62232/api/Lifetime")));
        }

        [Fact]
        public async Task HttpHandlerPerRequestLifetime()
        {
            Assert.Equal("Success", await ProcessWebRequest($"http://localhost:62232/LifetimeHandler.ashx?container={WebName}"));
            Assert.Equal("12345", await ProcessWebRequest("http://localhost:62232/LifetimeHandler.ashx?value=12345"));
            Assert.Equal("Default", await ProcessWebRequest("http://localhost:62232/LifetimeHandler.ashx"));
        }

        [Fact]
        public async Task MvcPerRequestLifetime()
        {
            Assert.Equal("Success", JsonConvert.DeserializeObject(await ProcessWebRequest($"http://localhost:62232/Lifetime/Initialize?container={WebName}")));
            Assert.Equal("12345", JsonConvert.DeserializeObject(await ProcessWebRequest("http://localhost:62232/Lifetime/SetValue?value=12345")));
            Assert.Equal("Default", JsonConvert.DeserializeObject(await ProcessWebRequest("http://localhost:62232/Lifetime/GetValue")));
        }

        [Fact]
        public async Task WebPagePerRequestLifetime()
        {
            Assert.Equal("Success", await ProcessWebRequest($"http://localhost:62232/LifetimePage.aspx?container={WebName}"));
            Assert.Equal("12345", await ProcessWebRequest("http://localhost:62232/LifetimePage.aspx?value=12345"));
            Assert.Equal("Default", await ProcessWebRequest("http://localhost:62232/LifetimePage.aspx"));
        }

        private async Task<string> ProcessWebRequest(string url)
        {
            var request = WebRequest.Create(url);
            var response = await request.GetResponseAsync();
            var stream = response.GetResponseStream();
            if (stream == null) return null;
            using (var reader = new StreamReader(stream))
            {
                return await reader.ReadToEndAsync();
            }
        }

        [Fact]
        public void TransientInstances()
        {
            ServiceContainer.Register<LifetimeObject>(ServiceLifetime.Transient);
            LifetimeObject result1 = null;
            LifetimeObject result2 = null;
            LifetimeObject result3 = null;
            Thread thread1 = new Thread(delegate ()
            {
                result1 = ServiceContainer.GetInstance<LifetimeObject>();
            });

            Thread thread2 = new Thread(delegate ()
            {
                result2 = ServiceContainer.GetInstance<LifetimeObject>();
                result3 = ServiceContainer.GetInstance<LifetimeObject>();
            });
            thread1.Start();
            thread2.Start();

            thread2.Join();
            thread1.Join();

            Assert.NotNull(result1);
            Assert.NotNull(result2);
            Assert.NotNull(result3);
            Assert.NotSame(result1, result2);
            Assert.NotSame(result3, result2);
        }

        #endregion
    }
}
