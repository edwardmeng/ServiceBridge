using System.Collections;
using System.Collections.Generic;
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
            ServiceContainer.Register<ICanChangeParameters, CanChangeParametersTarget>();
        }

        [Fact]
        public void GetInstance()
        {
            var instance = ServiceContainer.GetInstance<ILogger>();
            Assert.NotNull(instance);
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
    }
}
