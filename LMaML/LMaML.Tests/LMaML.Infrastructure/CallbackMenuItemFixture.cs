using System;
using LMaML.Infrastructure.Services.Interfaces;
using LMaML.Infrastructure.Util;
using NUnit.Framework;
using Telerik.JustMock;

namespace LMaML.Tests.LMaML.Infrastructure
{
    [TestFixture]
    public class CallbackMenuItemFixture
    {
        [Test]
        public void WhenNameNullExceptionThrown()
        {
            Assert.Throws<ArgumentNullException>(() => new CallbackMenuItem(null, null));
        }

        [Test]
        public void WhenSubItemsNullExceptionThrown()
        {
            Assert.Throws<ArgumentNullException>(() => new CallbackMenuItem(null, "Some name", null));
        }

        [Test]
        public void WhenASubItemNullExceptionThrown()
        {
            Assert.Throws<ArgumentNullException>(
                () => new CallbackMenuItem(null, "Some name", new CallbackMenuItem(null, "Some name"), null));
        }

        [Test]
        public void WhenCommandExecutedCallbackCalled()
        {
            // Arrange
            var called = false;
            var target = new CallbackMenuItem(() => called = true, "Some name");

            // Act
            target.Command.Execute(null);

            // Assert
            Assert.IsTrue(called);
        }

        [Test]
        public void WhenSubItemChangedParentChanged()
        {
            // Arrange
            var subItem = Mock.Create<IMenuItem>();
            var target = new CallbackMenuItem(null, "Some name", subItem);
            var raised = false;
            target.Changed += () => raised = true;

            // Act
            Mock.Raise(() => subItem.Changed += null);

            // Assert
            Assert.IsTrue(raised);
        }
    }
}
