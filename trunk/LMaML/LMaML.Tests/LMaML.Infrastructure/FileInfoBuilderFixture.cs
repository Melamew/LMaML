using System;
using System.IO;
using System.Linq;
using LMaML.Infrastructure.Util;
using NUnit.Framework;

namespace LMaML.Tests.LMaML.Infrastructure
{
    [TestFixture]
    public class FileInfoBuilderFixture
    {
        [Test]
        public void WhenBuildFileInfoNotNullTrueReturned()
        {
            // Arrange
            var target = new FileInfoBuilder();
            bool valid;

            // Act
            target.Build(GetFileInfo(), out valid);

            // Assert
            Assert.IsTrue(valid);
        }

        private static FileInfo GetFileInfo()
        {
            return new DirectoryInfo(Environment.CurrentDirectory).GetFiles().FirstOrDefault();
        }

        [Test]
        public void WhenBuildFileInfoNullFalseReturned()
        {
            // Arrange
            var target = new FileInfoBuilder();
            bool valid;

            // Act
            target.Build(null, out valid);

            // Assert
            Assert.IsFalse(valid);
        }
    }
}
