using System;
using NUnit.Framework;

namespace LMaML.Tests
{
    [TestFixture]
    public class GlobalHotkeyServiceFixture
    {
        [Test]
        public void WhenInitializedExceptionThrownBecauseWPFIsReallyNotTestableInAnyWayWhatsoever()
        {
            Assert.Throws<ArgumentNullException>(() => new GlobalHotkeyService());
        }
    }
}
