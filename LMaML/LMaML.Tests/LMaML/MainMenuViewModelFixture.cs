using LMaML.Infrastructure.Events;
using LMaML.Infrastructure.Services.Implementations;
using LMaML.Infrastructure.Services.Interfaces;
using LMaML.Tests.Helpers;
using LMaML.ViewModels;
using NUnit.Framework;
using Telerik.JustMock;

namespace LMaML.Tests.LMaML
{
    [TestFixture, RequiresSTA]
    public class MainMenuViewModelFixture
    {
        [Test]
        public void NullParameterTest()
        {
            TestHelper.NullParameterTest<MainMenuViewModel>();
        }

        [Test]
        public void WhenInitializedMenuItemsRequested()
        {
            // Arrange
            var menuServiceMock = Mock.Create<IMenuService>();
            var target = new Builder<MainMenuViewModel>().With(menuServiceMock);

            // Act
            target.Build();

            // Assert
            Mock.Assert(() => menuServiceMock.RootMenus);
            //menuServiceMock.Verify(x => x.RootMenus);
        }

        [Test]
        public void WhenInitializedMenuBuilt()
        {
            // Arrange
            var menuServiceMock = Mock.Create<IMenuService>();
            var itemMock = Mock.Create<IMenuItem>();
            var subItem = Mock.Create<IMenuItem>();
            Mock.Arrange(() => itemMock.SubItems).Returns(new[] {subItem});
            Mock.Arrange(() => menuServiceMock.RootMenus).Returns(new[] {itemMock});
            var builder = new Builder<MainMenuViewModel>().With(menuServiceMock);

            // Act
            var target = builder.Build();

            // Assert
            Assert.That(target.MenuItems.Count == 1);
        }

        [Test]
        public void WhenMenuChangedMenuRebuilt()
        {
            // Arrange
            var menuServiceMock = Mock.Create<IMenuService>();
            var publicTransport = Mock.Create<IPublicTransport>();
            var eventBus = new EventBus<IApplicationEvent>();
            Mock.Arrange(() => publicTransport.ApplicationEventBus).Returns(eventBus);
            var target = new Builder<MainMenuViewModel>().With(menuServiceMock).With(publicTransport).Build();
            Mock.Arrange(() => menuServiceMock.RootMenus).Returns(new[] { Mock.Create<IMenuItem>() });

            // Act
            eventBus.Send(new MainMenuChangedEvent());

            // Assert
            Assert.That(target.MenuItems.Count == 1);
        }
    }
}
