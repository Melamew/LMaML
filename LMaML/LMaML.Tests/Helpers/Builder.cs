using System;
using LMaML.Infrastructure.Events;
using LMaML.Infrastructure.Services.Interfaces;
using LMaML.Tests.Helpers;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.ObjectBuilder;
using Telerik.JustMock;
using iLynx.Common.WPF;

namespace LMaML.Tests
{
    public class Builder<T>
    {
        private readonly IUnityContainer container;

        public Builder()
        {
            container = new UnityContainer();
            container.AddExtension(new AutoMockingExtension());
            SetupDefaults();
        }

        [STAThread]
        private void SetupDefaults()
        {
            container.RegisterInstance<IDispatcher>(new DispatcherMock());
            container.RegisterInstance(container);
            var publicTransportMock = Mock.Create<IPublicTransport>();
            Mock.Arrange(() => publicTransportMock.ApplicationEventBus).Returns(Mock.Create<IEventBus<IApplicationEvent>>());
            Mock.Arrange(() => publicTransportMock.CommandBus).Returns(Mock.Create<ICommandBus>());
            container.RegisterInstance(publicTransportMock);
        }

        /// <summary>
        /// Withes the specified value.
        /// </summary>
        /// <typeparam name="TVal">The type of the val.</typeparam>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public Builder<T> With<TVal>(TVal value)
        {
            container.RegisterInstance(value);
            return this;
        }

        /// <summary>
        /// Builds this instance.
        /// </summary>
        /// <returns></returns>
        public T Build()
        {
            return container.Resolve<T>();
        }
    }

    public class DispatcherMock : IDispatcher
    {
        #region Implementation of IDispatcher

        /// <summary>
        /// Invokes the specified method.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="timeout">The timeout.</param>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        public object Invoke(Delegate method, TimeSpan timeout, params object[] args)
        {
            return method.DynamicInvoke(args);
        }

        /// <summary>
        /// Begins the invoke.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="args">The args.</param>
        public void BeginInvoke(Delegate method, params object[] args)
        {
            method.DynamicInvoke(args);
        }

        /// <summary>
        /// Invokes the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        public void Invoke(Action action)
        {
            action();
        }

        /// <summary>
        /// Invokes the specified action.
        /// </summary>
        /// <typeparam name="TParam">The type of the param.</typeparam>
        /// <param name="action">The action.</param>
        /// <param name="param">The param.</param>
        public void Invoke<TParam>(Action<TParam> action, TParam param)
        {
            action(param);
        }

        /// <summary>
        /// Invokes the specified func.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="func">The func.</param>
        /// <returns></returns>
        public TResult Invoke<TResult>(Func<TResult> func)
        {
            return func();
        }

        #endregion
    }


    #region Thanks to http://www.agileatwork.com/auto-mocking-unity-container-extension/
    public class AutoMockingExtension : UnityContainerExtension
    {
        protected override void Initialize()
        {
            var strategy = new AutoMockingBuilderStrategy(Container);

            Context.Strategies.Add(strategy, UnityBuildStage.PreCreation);
        }

        class AutoMockingBuilderStrategy : BuilderStrategy
        {
            private readonly IUnityContainer container;

            public AutoMockingBuilderStrategy(IUnityContainer container)
            {
                this.container = container;
            }

            public override void PreBuildUp(IBuilderContext context)
            {
                var key = context.OriginalBuildKey;

                if (key.Type.IsInterface && !container.IsRegistered(key.Type))
                {
                    context.Existing = CreateDynamicMock(key.Type);
                }
            }

            private static object CreateDynamicMock(Type type)
            {
                return TestHelper.MakeMock(type);
            }
        }
    }
    #endregion
}
