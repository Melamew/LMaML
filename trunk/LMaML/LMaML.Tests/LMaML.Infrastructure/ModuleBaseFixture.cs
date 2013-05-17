using System;
using System.Collections.Generic;
using System.Linq;
using LMaML.Infrastructure;
using LMaML.Infrastructure.Services.Interfaces;
using LMaML.Tests.Helpers;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using Telerik.JustMock;

namespace LMaML.Tests.LMaML.Infrastructure
{
    [TestFixture]
    public class ModuleBaseFixture
    {
        public class TestModule : ModuleBase
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ModuleBase" /> class.
            /// </summary>
            /// <param name="container">The container.</param>
            public TestModule(IUnityContainer container) : base(container)
            {

            }

            /// <summary>
            /// Gets the actual container.
            /// </summary>
            /// <value>
            /// The actual container.
            /// </value>
            public IUnityContainer ActualContainer { get { return base.Container; } }

            private readonly List<string> callOrder = new List<string>();

            public IEnumerable<string> CallOrder
            {
                get { return callOrder; }
            }

            protected override void AddResources()
            {
                callOrder.Add("AddResources");
            }

            protected override void RegisterTypes()
            {
                callOrder.Add("RegisterTypes");
            }

            protected override void RegisterViews(IRegionManagerService regionManagerService)
            {
                callOrder.Add("RegisterViews");
            }

            public void AddResource(string resource)
            {
                AddResources(resource);
            }

            public void AddResource(Uri resource)
            {
                AddResources(resource);
            }
        }

        [Test]
        public void NullParameterTest()
        {
            TestHelper.NullParameterTest<TestModule>();
        }

        [Test]
        public void WhenInitializedContainerSet()
        {
            // Arrange
            var containerMock = Mock.Create<IUnityContainer>();
            
            // Act
            var target = new Builder<TestModule>().With(containerMock).Build();

            // Assert
            Assert.AreSame(containerMock, target.ActualContainer);
        }

        [Test]
        public void WhenInitializeCallOrderCorrect()
        {
            // Arrange
            var correctCallOrder = new[] {"AddResources", "RegisterTypes", "RegisterViews"};
            var target = new Builder<TestModule>().Build();
            
            // Act
            target.Initialize();

            // Assert
            Assert.IsTrue(correctCallOrder.SequenceEqual(target.CallOrder));
        }

        [Test]
        public void WhenAddResourcesStringMergeDictionaryServiceUsed()
        {
            var mockService = Mock.Create<IMergeDictionaryService>();
            var target = new Builder<TestModule>().With(mockService).Build();

            target.AddResource("file.something");
            target.AddResource(new Uri(Environment.CurrentDirectory + "\\file.something"));

            Mock.Assert(() => mockService.AddResource(Arg.IsAny<Uri>()), Occurs.AtLeast(2));
        }
    }
}
