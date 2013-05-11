using System;
using System.IO;
using LMaML.FMOD;
using LMaML.Infrastructure.Audio;
using LMaML.Tests.Helpers;
using NUnit.Framework;

namespace LMaML.Tests.LMaML.FMOD
{
    [TestFixture]
    public class AudioPlayerFixture
    {
        [Test]
        public void NullParameterCheck()
        {
            TestHelper.NullParameterTest<AudioPlayer>();
        }

        [Test]
        public void WhenCreateChannelChannelCreated()
        {
            IChannel result = null;
            try
            {
                // Arrange
                var target = new Builder<AudioPlayer>().Build();

                // Act
                result = target.CreateChannel(Path.Combine(Environment.CurrentDirectory, "Silence.mp3"));

                // Assert
                Assert.IsInstanceOf<AudioChannel>(result);
            }
            finally
            {
                Assert.IsNotNull(result);
                result.Dispose();
            }
        }

        [Test]
        public void Todo()
        {
            Assert.Inconclusive("Most of the error cases should probably be implemented through function tests");
        }
    }
}
