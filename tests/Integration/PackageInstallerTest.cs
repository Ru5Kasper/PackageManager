using NUnit.Framework;
using Moq;
using PackageManager.Core;
using PackageManager.Core.Contracts;
using PackageManager.Models;
using PackageManager.Enums;
using PackageManager.Models.Contracts;
using PackageManager.Repositories.Contracts;
using PackageManager.Info.Contracts;

namespace PackageManager.Tests.Integration
{
    public class PackageInstallerTests
    {
        [Test]
        public void Install_PerformOperation_CallsDownloader()
        {
            var downloader = new Mock<IDownloader>();
            var repository = new Mock<IRepository<IPackage>>();
            repository.Setup(r => r.GetAll()).Returns(new List<IPackage>());

            var project = new Mock<IProject>();
            project.Setup(p => p.Location).Returns("C:\\Test");
            project.Setup(p => p.PackageRepository).Returns(repository.Object);

            var installer = new PackageInstaller(downloader.Object, project.Object);

            var package = new Package(
                "Test",
                new PackageVersion(1, 0, 0, VersionType.final)
            );

            installer.Operation = InstallerOperation.Install;
            installer.PerformOperation(package);

            downloader.Verify(d => d.Download("Test"), Times.AtLeastOnce);
        }
    }
}
