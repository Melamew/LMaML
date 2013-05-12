using System.Linq;
using LMaML.Infrastructure.Events;
using LMaML.Infrastructure.Services.Implementations;
using LMaML.Infrastructure.Services.Interfaces;
using LMaML.Tests.Helpers;
using NUnit.Framework;
using Telerik.JustMock;

namespace LMaML.Tests.LMaML.Infrastructure
{
    [TestFixture]
    public class MenuServiceFixture
    {
        [Test]
        public void NullParameterTest()
        {
            TestHelper.NullParameterTest<MenuService>();
        }

        [Test]
        public void WhenRegisterChangedEventRaised()
        {
            // Arrange
            IEventBus<IApplicationEvent> eventBus;
            var publicTransportMock = TestHelper.MakePublicTransportMock(out eventBus);
            var target = new Builder<MenuService>().With(publicTransportMock).Build();

            // Act
            target.Register(Mock.Create<IMenuItem>());

            // Assert
            Mock.Assert(() => eventBus.Send(Arg.IsAny<MainMenuChangedEvent>()));
        }

        [Test]
        public void WhenRegisterSameNameTwiceIsReplaced()
        {
            // Arrange
            var target = new Builder<MenuService>().Build();
            var item1 = Mock.Create<IMenuItem>();
            var item2 = Mock.Create<IMenuItem>();
            const string name = "Name";
            Mock.Arrange(() => item1.Name).Returns(name);
            Mock.Arrange(() => item2.Name).Returns(name);

            // Act
            target.Register(item1);
            target.Register(item2);

            // Assert
            Assert.AreEqual(item2, target.RootMenus.FirstOrDefault(x => x.Name == name));
        }

        [Test]
        public void WhenMenuItemChangedMenuChangedRaised()
        {
            // Arrange
            IEventBus<IApplicationEvent> eventBus;
            var publicTransportMock = TestHelper.MakePublicTransportMock(out eventBus);
            var target = new Builder<MenuService>().With(publicTransportMock).Build();
            var item = Mock.Create<IMenuItem>();
            target.Register(item);

            // Act
            Mock.Raise(() => item.Changed += null);

            // Assert
            Mock.Assert(() => eventBus.Send(Arg.IsAny<MainMenuChangedEvent>()));
        }
    }
}
