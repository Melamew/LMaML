using LMaML.Infrastructure.Services.Implementations;
using LMaML.Tests.Helpers;
using NUnit.Framework;

namespace LMaML.Tests.LMaML.Infrastructure
{
    [TestFixture]
    public class RegionManagerServiceFixture
    {
        [Test]
        public void NullParameterTest()
        {
            TestHelper.NullParameterTest<RegionManagerService>();
        }

        [Test]
        public void SemiUntestable()
        {
            Assert.Inconclusive(
                "Since the built in Prism RegionManager extensions rely " +
                "heavily on ServiceLocator.Current to get a reference " +
                "to IRegionViewRegistry in order to then subsequently add " +
                "views to that instead of the RegionManager, this class is " +
                "pretty much untestable (Since it uses those exact methods)");
        }
    }
}
