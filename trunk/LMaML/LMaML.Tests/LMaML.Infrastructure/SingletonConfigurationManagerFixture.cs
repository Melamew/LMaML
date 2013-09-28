using System;
using System.Linq;
using LMaML.Infrastructure.Services.Implementations;
using LMaML.Tests.Helpers;
using NUnit.Framework;
using iLynx.Common.Configuration;

namespace LMaML.Tests.LMaML.Infrastructure
{
    [TestFixture]
    public class SingletonConfigurationManagerFixture
    {
        [Test]
        public void NullParameterTest()
        {
            TestHelper.NullParameterTest<SingletonConfigurationManager>();
        }

        [Test]
        public void WhenGetValueDoesNotExistNewOneCreatedWithDefaultValue()
        {
            // Arrange
            var target = new Builder<SingletonConfigurationManager>().Build();
            const int expected = 25;

            // Act
            var value = target.GetValue("Something", expected, "Some Category");

            // Assert
            Assert.AreEqual(expected, value.Value);
        }

        [Test]
        public void WhenGetValueExistsOpenReturned()
        {
            // Arrange
            var target = new Builder<SingletonConfigurationManager>().Build();

            // Act
            var value = target.GetValue("Something2", 23, "Category");
            var otherValue = target.GetValue("Something2", 25, "Category");

            // Assert
            Assert.AreSame(value, otherValue);
        }

        private static void EnsureValuesExist(IConfigurationManager manager, int count, string category = null)
        {
            for (var i = 0; i < count; ++i)
                manager.GetValue("Something" + i, i, category);
        }

        [Test]
        public void WhenGetLoadedValuesAreReturnedForCategory()
        {
            var target = new Builder<SingletonConfigurationManager>().Build();
            EnsureValuesExist(target, 10, "Category");
            EnsureValuesExist(target, 5, "Other category");
            // Act
            var values = target.GetLoadedValues();

            // Assert
            Assert.AreEqual(values.Count(), 10);
        }

        [Test]
        public void WhenGetCategoriesReturned()
        {
            var target = new Builder<SingletonConfigurationManager>().Build();
            EnsureValuesExist(target, 10, "Category");
            EnsureValuesExist(target, 5, "Other category");

            var categories = target.GetCategories();

            // Assert - Note that the singletonconfigurationmanager works on a static class behind the scenes,
            // which in turn actually stores values and reloads them, thus, we may have leftovers from other tests.
            Assert.IsTrue(categories.Count() >= 2);
        }

        [Test]
        public void WhenValueExistsButIsOfDifferentTypeThanRequestedExceptionThrown()
        {
            // Arrange
            var target = new Builder<SingletonConfigurationManager>().Build();
            const string key = "Some Key";
            
            // Act1
            target.GetValue(key, "Some value");

            // Act2 / Assert
            Assert.Throws<InvalidCastException>(() => target.GetValue(key, 25));
        }
    }
}
