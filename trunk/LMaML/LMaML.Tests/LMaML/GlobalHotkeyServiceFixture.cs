using System;
using NUnit.Framework;
using iLynx.Common;
using iLynx.Common.WPF;

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
