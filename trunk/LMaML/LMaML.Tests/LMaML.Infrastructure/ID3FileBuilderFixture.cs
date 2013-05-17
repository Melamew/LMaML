using System.IO;
using LMaML.Infrastructure.Util;
using NUnit.Framework;

namespace LMaML.Tests.LMaML.Infrastructure
{
    [TestFixture]
    public class ID3FileBuilderFixture
    {
        [Test]
        public void WhenBuildValidReturnsId3FileIsValid()
        {
// ReSharper disable AssignNullToNotNullAttribute
            var invalidFile = new FileInfo(Path.GetFileName(typeof (ID3FileBuilderFixture).Assembly.CodeBase));
// ReSharper restore AssignNullToNotNullAttribute
            var target = new ID3FileBuilder();
            bool valid;

            var result =target.Build(invalidFile, out valid);

            Assert.AreEqual(result.IsValid, valid);
        }
    }
}
