using System;
using System.IO;
using LMaML.Infrastructure.Audio;
using LMaML.Infrastructure.Domain;
using LMaML.Infrastructure.Domain.Concrete;
using LMaML.Infrastructure.Util;
using LMaML.Tests.Helpers;
using NUnit.Framework;
using Telerik.JustMock;

namespace LMaML.Tests.LMaML.Infrastructure
{
    [TestFixture]
    public class StorableTaggedFileBuilderFixture
    {
        [Test]
        public void NullParameterCheck()
        {
            TestHelper.NullParameterTest<StorableTaggedFileBuilder>();
        }

        [Test]
        public void WhenBuildSourceBuilderBuild()
        {
            // Arrange
            var sourceBuilder = Mock.Create<IInfoBuilder<ID3File>>();
            var target = new Builder<StorableTaggedFileBuilder>().With(sourceBuilder).Build();
            bool stuff;

            // Act
            target.Build(new FileInfo(Path.Combine(Environment.CurrentDirectory, "Silence.mp3")), out stuff);

            // Assert
            Mock.Assert(() => sourceBuilder.Build(Arg.IsAny<FileInfo>(), out stuff));
        }

        private class BuilderMock : IInfoBuilder<ID3File>
        {
            private ID3File f;
            private bool v;
            public BuilderMock Returns(ID3File file)
            {
                f = file;
                return this;
            }

            public BuilderMock Sets(bool valid)
            {
                v = valid;
                return this;
            }

            #region Implementation of IInfoBuilder<out ID3File>

            /// <summary>
            /// Builds the specified info.
            /// </summary>
            /// <param name="info">The info.</param>
            /// <param name="valid">if set to <c>true</c> [valid].</param>
            /// <returns></returns>
            public ID3File Build(FileInfo info, out bool valid)
            {
                valid = v;
                return f;
            }

            #endregion
        }

        [Test]
        public void WhenSourceValidStorableTaggedFileReturned()
        {
            // Arrange
            var sourceBuilder = new BuilderMock().Returns(new ID3File(Path.Combine(Environment.CurrentDirectory, "Silence.mp3"))).Sets(true);
            bool valid;
            var target = new Builder<StorableTaggedFileBuilder>().With<IInfoBuilder<ID3File>>(sourceBuilder).Build();

            // Act
            var result = target.Build(Arg.IsAny<FileInfo>(), out valid);

            // Assert
            Assert.IsInstanceOf<StorableTaggedFile>(result);
        }

        [Test]
        public void WhenSourceBuildNotValidNullReturned()
        {
            // Arrange
            var sourceBuilder = new BuilderMock().Sets(false).Returns(null);
            bool valid;
            var target = new Builder<StorableTaggedFileBuilder>().With<IInfoBuilder<ID3File>>(sourceBuilder).Build();

            // Act
            var result = target.Build(Arg.IsAny<FileInfo>(), out valid);

            // Assert
            Assert.IsNull(result);
        }
    }
}
