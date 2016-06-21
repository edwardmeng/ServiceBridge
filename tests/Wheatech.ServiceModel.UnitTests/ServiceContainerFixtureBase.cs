using System.Collections;
using System.Collections.Generic;
using System.Threading;
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
        }

        [Fact]
        public void AskingForInvalidComponentShouldRaiseActivationException()
        {
            Assert.Throws<ActivationException>(() => ServiceContainer.GetInstance<IDictionary>());
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

        [Fact]
        public void CanInjectionConstructor()
        {
            var o = ServiceContainer.GetInstance<ObjectWithInjection>();

            Assert.NotNull(o.InjectionFromConstructor);
            Assert.Null(o.NotInjectionFromConstructor);
        }

        [Fact]
        public void CanInjectionMethod()
        {
            var o = ServiceContainer.GetInstance<ObjectWithInjection>();
            Assert.NotNull(o.InjectionFromMethod);
        }

        [Fact]
        public void CanInjectionProperty()
        {
            var o = ServiceContainer.GetInstance<ObjectWithInjection>();

            Assert.NotNull(o.InjectionFromProperty);
            Assert.Null(o.NotInjectionFromProperty);
        }

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

        [Fact]
        public void SingletonInThreads()
        {
            ServiceContainer.Current.Registering += SingletonRegistering;

            ServiceContainer.Register<LifetimeObject>();
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

            ServiceContainer.Current.Registering -= SingletonRegistering;
        }

        [Fact]
        public void SingletonInstances()
        {
            ServiceContainer.Current.Registering += SingletonRegistering;
            ServiceContainer.Register<LifetimeObject>();
            LifetimeObject result1 = ServiceContainer.GetInstance<LifetimeObject>();
            LifetimeObject result2 = ServiceContainer.GetInstance<LifetimeObject>();
            Assert.NotNull(result1);
            Assert.Same(result1, result2);

            ServiceContainer.Current.Registering -= SingletonRegistering;
        }

        private void SingletonRegistering(object sender, ServiceRegisterEventArgs e)
        {
            e.Lifetime = ServiceLifetime.Singleton;
        }

        [Fact]
        public void PerThreadInstances()
        {
            ServiceContainer.Current.Registering += PerThreadRegistering;

            ServiceContainer.Register<LifetimeObject>();
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

            ServiceContainer.Current.Registering -= PerThreadRegistering;
        }

        private void PerThreadRegistering(object sender, ServiceRegisterEventArgs e)
        {
            e.Lifetime = ServiceLifetime.PerThread;
        }

        [Fact]
        public void TransientInstances()
        {
            ServiceContainer.Current.Registering += TransientRegistering;
            ServiceContainer.Register<LifetimeObject>();
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
            ServiceContainer.Current.Registering -= TransientRegistering;
        }

        private void TransientRegistering(object sender, ServiceRegisterEventArgs e)
        {
            e.Lifetime = ServiceLifetime.Transient;
        }

        #endregion
    }
}
