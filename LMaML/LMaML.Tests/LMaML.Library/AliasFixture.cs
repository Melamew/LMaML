using System;
using LMaML.Library.ViewModels;
using NUnit.Framework;

namespace LMaML.Tests.LMaML.Library
{
    [TestFixture]
    public class AliasFixture
    {
        [Test]
        public void WhenOriginalNullExceptionThrown()
        {
            Assert.Throws<ArgumentNullException>(() => new Alias<string>(null, "Something"));
        }

        [Test]
        public void WhenValueNullExceptionThrown()
        {
            Assert.Throws<ArgumentNullException>(() => new Alias<string>("Something", null));
        }

        [Test]
        public void WhenInitializedValuesSet()
        {
            // Arrange
            const string first = "First";
            const string second = "Second";

            // Act
            var target = new Alias<string>(first, second);

            // Assert
            Assert.AreEqual(first, target.Original);
            Assert.AreEqual(second, target.Value);
        }

        [Test]
        public void WhenToStringAliasToStringReturned()
        {
            // Arrange
            const string first = "First";
            const string second = "Second";

            // Act
            var target = new Alias<string>(first, second);

            // Assert
            Assert.AreEqual(second, target.ToString());
        }

        [Test]
        public void WhenGetHashCodeOriginalHashCodeReturned()
        {
            // Arrange
            const string first = "First";
            const string second = "second";

            // Act
            var target = new Alias<string>(first, second);

            // Assert
            Assert.AreEqual(first.GetHashCode(), target.GetHashCode());
        }
    }
}
