using System;
using LMaML.FMOD;
using LMaML.Tests.Helpers;
using NUnit.Framework;

namespace LMaML.Tests.LMaML.FMOD
{
    [TestFixture]
    public class AudioChannelFixture
    {
        [Test]
        public void NullParameterCheck()
        {
            TestHelper.NullParameterTest<AudioChannel>();
        }

        [Test]
        public void Todo()
        {
            Assert.Inconclusive("Most of the tests that can be made for this class require actual instances of FMOD sound and system objects, so should be made function tests");
        }

        //[Test]
        //public void WhenFFTStereoCalledWithNonPowerOfTwoExceptionThrown()
        //{
        //    // Arrange
        //    var target = new Builder<AudioChannel>().Build();

        //    // Act / Assert
        //    Assert.Throws<NotSupportedException>(() => target.FFTStereo(65));
        //}

        //[Test]
        //public void WhenFFTCalledWithNonPowerOfTwoExceptionThrown()
        //{
        //    // Arrange
        //    var target = new Builder<AudioChannel>().Build();

        //    // Act / Assert
        //    Assert.Throws<NotSupportedException>(() => target.FFT(65));
        //}
    }
}
