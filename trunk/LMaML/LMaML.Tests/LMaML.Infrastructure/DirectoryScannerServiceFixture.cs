using System;
using LMaML.Infrastructure.Services.Implementations;
using LMaML.Infrastructure.Util;
using LMaML.Tests.Helpers;
using NUnit.Framework;
using Telerik.JustMock;

namespace LMaML.Tests.LMaML.Infrastructure
{
    [TestFixture]
    public class DirectoryScannerServiceFixture
    {
        [Test]
        public void NullParameterTest()
        {
            TestHelper.NullParameterTest<DirectoryScannerService<int>>();
        }

        [Test]
        public void WhenCancelAllScannerCancelled()
        {
            // Arrange
            var scannerMock = Mock.Create<IAsyncFileScanner<int>>();
            var target = new Builder<DirectoryScannerService<int>>().With(scannerMock).Build();

            // Act
            target.CancelAll();

            // Assert
            Mock.Assert(() => scannerMock.Cancel());
        }

        [Test]
        public void WhenScanScannerIsStarted()
        {
            // Arrange
            var scannerMock = Mock.Create<IAsyncFileScanner<int>>();
            var target = new Builder<DirectoryScannerService<int>>().With(scannerMock).Build();

            // Act
            target.Scan("Somewhere");

            // Assert
            Mock.Assert(() => scannerMock.Execute(Arg.IsAny<FileScannerArgs>(), Arg.IsAny<Action<ScanCompletedEventArgs<int>>>()));
        }

        [Test]
        public void WhenScanRootNullOrEmptyExceptionThrown()
        {
            // Arrange
            var target = new Builder<DirectoryScannerService<int>>().Build();

            // Act / Assert
            Assert.Throws<ArgumentNullException>(() => target.Scan(null));
        }

        [Test]
        public void WhenScanerCompletedScanCompletedRaised()
        {
            // Arrange
            var scannerMock = Mock.Create<IAsyncFileScanner<int>>();
            Action<ScanCompletedEventArgs<int>> callback = null;
            Mock.Arrange(() => scannerMock.Execute(Arg.IsAny<FileScannerArgs>(), Arg.IsAny<Action<ScanCompletedEventArgs<int>>>()))
                .DoInstead<FileScannerArgs, Action<ScanCompletedEventArgs<int>>>((args,
                                                                                  action) => callback = action);
            var target = new Builder<DirectoryScannerService<int>>().With(scannerMock).Build();
            var raised = false;
            target.ScanCompleted += (sender, args) => raised = true;
            target.Scan("Somewhere");
            Assert.IsNotNull(callback);
            
            // Act
            callback(new ScanCompletedEventArgs<int>(Guid.Empty));

            // Assert
            Assert.IsTrue(raised);
        }

        [Test]
        public void WhenScannerProgressIsRaised()
        {
            // Arrange
            var scannerMock = Mock.Create<IAsyncFileScanner<int>>();
            var target = new Builder<DirectoryScannerService<int>>().With(scannerMock).Build();
            var raised = false;
            target.ScanProgress += d => raised = true;

            // Act
            Mock.Raise(() => scannerMock.Progress += null, scannerMock, 50d);

            // Assert
            Assert.IsTrue(raised);
        }

        [Test]
        public void Todo()
        {
            Assert.Inconclusive("Make DirectoryScannerService capable of multiple scans again");
        }
    }
}
