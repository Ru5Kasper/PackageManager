using NUnit.Framework;
using PackageManager.Core;
using PackageManager.Core.Contracts;
using PackageManager.Models.Contracts;
using PackageManager.Repositories.Contracts;
using PackageManager.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using Moq;

namespace PackageManager.Tests.Core
{
    [TestFixture]
    public class PackageInstallerTests
    {
        private Mock<IDownloader> mockDownloader;
        private Mock<IProject> mockProject;
        private Mock<IRepository<IPackage>> mockPackageRepository;
        private PackageInstaller installer;

        [SetUp]
        public void SetUp()
        {
            mockDownloader = new Mock<IDownloader>();
            mockProject = new Mock<IProject>();
            mockPackageRepository = new Mock<IRepository<IPackage>>();
            
            mockProject.Setup(p => p.PackageRepository).Returns(mockPackageRepository.Object);
            mockProject.Setup(p => p.Location).Returns("C:\\TestProject");
            
            installer = new PackageInstaller(mockDownloader.Object, mockProject.Object);
        }

        [Test]
        public void Constructor_ShouldSetDownloaderLocation()
        {
            // Arrange
            string expectedLocation = "C:\\TestProject\\my_modules";

            // Assert
            mockDownloader.VerifySet(d => d.Location = expectedLocation, Times.Once);
        }

        [Test]
        public void Constructor_ShouldCallRestorePackages()
        {
            // Arrange
            var packages = new List<IPackage>
            {
                new Mock<IPackage>().Object
            };
            
            mockPackageRepository.Setup(r => r.GetAll()).Returns(packages);

            // Act
            var newInstaller = new PackageInstaller(mockDownloader.Object, mockProject.Object);

            // Assert
            mockPackageRepository.Verify(r => r.GetAll(), Times.AtLeastOnce);
        }

        [Test]
        public void BasicFolder_ShouldReturnCorrectValue()
        {
            // Act
            var result = installer.BasicFolder;

            // Assert
            Assert.AreEqual("my_modules", result);
        }

        [Test]
        public void PerformOperation_ShouldInstallPackage_WhenOperationIsInstall()
        {
            // Arrange
            installer.Operation = InstallerOperation.Install;
            
            var mockPackage = new Mock<IPackage>();
            mockPackage.Setup(p => p.Name).Returns("TestPackage");
            mockPackage.Setup(p => p.Url).Returns("1.0.0-alpha");
            mockPackage.Setup(p => p.Dependencies).Returns(new List<IPackage>());

            // Act
            installer.PerformOperation(mockPackage.Object);

            // Assert
            mockPackageRepository.Verify(r => r.Add(mockPackage.Object), Times.Once);
            mockDownloader.Verify(d => d.Remove("TestPackage"), Times.Once);
            mockDownloader.Verify(d => d.Download("TestPackage"), Times.Once);
            mockDownloader.Verify(d => d.Download("TestPackage\\1.0.0-alpha"), Times.Once);
        }

        [Test]
        public void PerformOperation_ShouldUninstallPackage_WhenOperationIsUninstall()
        {
            // Arrange
            installer.Operation = InstallerOperation.Uninstall;
            
            var mockPackage = new Mock<IPackage>();
            mockPackage.Setup(p => p.Name).Returns("TestPackage");
            mockPackage.Setup(p => p.Dependencies).Returns(new List<IPackage>());
            
            mockPackageRepository.Setup(r => r.Delete(mockPackage.Object)).Returns(mockPackage.Object);

            // Act
            installer.PerformOperation(mockPackage.Object);

            // Assert
            mockPackageRepository.Verify(r => r.Delete(mockPackage.Object), Times.Once);
            mockDownloader.Verify(d => d.Remove("TestPackage"), Times.Once);
        }

        [Test]
        public void PerformOperation_ShouldUpdatePackage_WhenOperationIsUpdate()
        {
            // Arrange
            installer.Operation = InstallerOperation.Update;
            
            var mockPackage = new Mock<IPackage>();
            mockPackage.Setup(p => p.Name).Returns("TestPackage");
            mockPackage.Setup(p => p.Url).Returns("2.0.0-alpha");
            mockPackage.Setup(p => p.Dependencies).Returns(new List<IPackage>());
            
            mockPackageRepository.Setup(r => r.Update(mockPackage.Object)).Returns(true);

            // Act
            installer.PerformOperation(mockPackage.Object);

            // Assert
            mockPackageRepository.Verify(r => r.Update(mockPackage.Object), Times.Once);
            mockDownloader.Verify(d => d.Remove("TestPackage"), Times.Once);
            mockDownloader.Verify(d => d.Download("TestPackage"), Times.Once);
            mockDownloader.Verify(d => d.Download("TestPackage\\2.0.0-alpha"), Times.Once);
        }

        [Test]
        public void PerformOperation_ShouldNotUpdateDownloader_WhenUpdateReturnsFalse()
        {
            // Arrange
            installer.Operation = InstallerOperation.Update;
            
            var mockPackage = new Mock<IPackage>();
            mockPackage.Setup(p => p.Name).Returns("TestPackage");
            mockPackage.Setup(p => p.Dependencies).Returns(new List<IPackage>());
            
            mockPackageRepository.Setup(r => r.Update(mockPackage.Object)).Returns(false);

            // Act
            installer.PerformOperation(mockPackage.Object);

            // Assert
            mockPackageRepository.Verify(r => r.Update(mockPackage.Object), Times.Once);
            mockDownloader.Verify(d => d.Remove(It.IsAny<string>()), Times.Never);
            mockDownloader.Verify(d => d.Download(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void PerformOperation_ShouldProcessDependencies_WhenPackageHasDependencies()
        {
            // Arrange
            installer.Operation = InstallerOperation.Install;
            
            var mockDependency = new Mock<IPackage>();
            mockDependency.Setup(d => d.Name).Returns("Dependency");
            mockDependency.Setup(d => d.Url).Returns("1.0.0-alpha");
            mockDependency.Setup(d => d.Dependencies).Returns(new List<IPackage>());
            
            var mockPackage = new Mock<IPackage>();
            mockPackage.Setup(p => p.Name).Returns("TestPackage");
            mockPackage.Setup(p => p.Url).Returns("2.0.0-alpha");
            mockPackage.Setup(p => p.Dependencies).Returns(new List<IPackage> { mockDependency.Object });

            // Act
            installer.PerformOperation(mockPackage.Object);

            // Assert
            mockPackageRepository.Verify(r => r.Add(mockPackage.Object), Times.Once);
            mockPackageRepository.Verify(r => r.Add(mockDependency.Object), Times.Once);
            mockDownloader.Verify(d => d.Download("TestPackage"), Times.Once);
            mockDownloader.Verify(d => d.Download("Dependency"), Times.Once);
        }

        [Test]
        public void PerformOperation_ShouldThrowNotImplementedException_WhenOperationIsInvalid()
        {
            // Arrange
            installer.Operation = (InstallerOperation)999; // Invalid enum value
            
            var mockPackage = new Mock<IPackage>();

            // Act & Assert
            Assert.Throws<NotImplementedException>(() =>
                installer.PerformOperation(mockPackage.Object));
        }
    }
}