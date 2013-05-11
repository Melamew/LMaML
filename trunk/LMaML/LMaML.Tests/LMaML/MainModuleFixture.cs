using System;
using LMaML.Infrastructure;
using LMaML.Infrastructure.Services.Interfaces;
using NUnit.Framework;
using Telerik.JustMock;

namespace LMaML.Tests.LMaML
{
    [TestFixture]
    public class MainModuleFixture
    {
        [Test]
        public void WhenInitializedViewsRegistered()
        {
            // Arrange
            var regionManagerMock = Mock.Create<IRegionManagerService>();
            var target = new Builder<MainModule>().With(regionManagerMock).Build();
            
            // Act
            target.Initialize();
            

            // Assert
            Mock.Assert(() => regionManagerMock.RegisterViewWithRegion(RegionNames.HeaderRegion, Arg.IsAny<Func<object>>()));
            Mock.Assert(() => regionManagerMock.RegisterViewWithRegion(RegionNames.StatusRegion, Arg.IsAny<Func<object>>()));
        }
    }
}
