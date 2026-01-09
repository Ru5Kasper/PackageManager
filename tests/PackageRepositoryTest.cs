using NUnit.Framework;
using PackageManager.Repositories;
using PackageManager.Info.Contracts;
using PackageManager.Models.Contracts;
using PackageManager.Models;
using PackageManager.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using Moq;

namespace PackageManager.Tests.Repositories
{
    [TestFixture]
    public class PackageRepositoryTests
    {
        private Mock<ILogger> mockLogger;
        private ICollection<IPackage> packages;
        private PackageRepository repository;

        [SetUp]
        public void SetUp()
        {
            mockLogger = new Mock<ILogger>();
            packages = new HashSet<IPackage>();
            repository = new PackageRepository(mockLogger.Object, packages);
        }

        [Test]
        public void Constructor_ShouldThrowArgumentNullException_WhenLoggerIsNull()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new PackageRepository(null, packages));
        }

        [Test]
        public void Constructor_ShouldInitializeEmptyCollection_WhenPackagesIsNull()
        {
            // Arrange & Act
            var repo = new PackageRepository(mockLogger.Object, null);

            // Assert
            Assert.IsNotNull(repo.GetAll());
            Assert.AreEqual(0, repo.GetAll().Count());
        }

        [Test]
        public void Add_ShouldThrowArgumentNullException_WhenPackageIsNull()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                repository.Add(null));
        }

        [Test]
        public void Add_ShouldAddPackage_WhenPackageDoesNotExist()
        {
            // Arrange
            var mockPackage = new Mock<IPackage>();
            mockPackage.Setup(p => p.Name).Returns("NewPackage");
            
            var mockVersion = new Mock<IVersion>();
            mockPackage.Setup(p => p.Version).Returns(mockVersion.Object);
            mockPackage.Setup(p => p.CompareTo(It.IsAny<IPackage>())).Returns(1);

            // Act
            repository.Add(mockPackage.Object);

            // Assert
            mockLogger.Verify(l => l.Log(It.IsAny<string>()), Times.AtLeastOnce);
        }

        [Test]
        public void Add_ShouldNotAddPackage_WhenSameVersionExists()
        {
            // Arrange
            var existingPackage = new Package("TestPackage", 
                new PackageVersion(1, 0, 0, VersionType.alpha));
            
            packages.Add(existingPackage);

            var newPackage = new Package("TestPackage", 
                new PackageVersion(1, 0, 0, VersionType.alpha));

            // Act
            repository.Add(newPackage);

            // Assert
            mockLogger.Verify(l => l.Log(It.Is<string>(s => s.Contains("same version"))), Times.AtLeastOnce);
        }

        [Test]
        public void Delete_ShouldThrowArgumentNullException_WhenPackageIsNull()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                repository.Delete(null));
        }

        [Test]
        public void Delete_ShouldThrowArgumentNullException_WhenPackageNotFound()
        {
            // Arrange
            var mockPackage = new Mock<IPackage>();
            mockPackage.Setup(p => p.Name).Returns("NonExistentPackage");

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                repository.Delete(mockPackage.Object));
        }

        [Test]
        public void Delete_ShouldRemovePackage_WhenNoDependenciesExist()
        {
            // Arrange
            var package = new Package("TestPackage", 
                new PackageVersion(1, 0, 0, VersionType.alpha));
            
            packages.Add(package);

            // Act
            var result = repository.Delete(package);

            // Assert
            Assert.AreEqual(package, result);
            Assert.AreEqual(0, packages.Count);
        }

        [Test]
        public void Update_ShouldThrowArgumentNullException_WhenPackageIsNull()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                repository.Update(null));
        }

        [Test]
        public void Update_ShouldThrowArgumentNullException_WhenPackageNotFound()
        {
            // Arrange
            var mockPackage = new Mock<IPackage>();
            mockPackage.Setup(p => p.Name).Returns("NonExistentPackage");
            
            var mockVersion = new Mock<IVersion>();
            mockPackage.Setup(p => p.Version).Returns(mockVersion.Object);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                repository.Update(mockPackage.Object));
        }

        [Test]
        public void Update_ShouldReturnTrue_WhenNewVersionIsHigher()
        {
            // Arrange
            var existingPackage = new Package("TestPackage", 
                new PackageVersion(1, 0, 0, VersionType.alpha));
            
            packages.Add(existingPackage);

            var newPackage = new Package("TestPackage", 
                new PackageVersion(2, 0, 0, VersionType.alpha));

            // Act
            var result = repository.Update(newPackage);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void Update_ShouldThrowArgumentException_WhenNewVersionIsLower()
        {
            // Arrange
            var existingPackage = new Package("TestPackage", 
                new PackageVersion(2, 0, 0, VersionType.alpha));
            
            packages.Add(existingPackage);

            var newPackage = new Package("TestPackage", 
                new PackageVersion(1, 0, 0, VersionType.alpha));

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                repository.Update(newPackage));
        }

        [Test]
        public void GetAll_ShouldReturnAllPackages()
        {
            // Arrange
            var package1 = new Package("Package1", 
                new PackageVersion(1, 0, 0, VersionType.alpha));
            var package2 = new Package("Package2", 
                new PackageVersion(2, 0, 0, VersionType.beta));
            
            packages.Add(package1);
            packages.Add(package2);

            // Act
            var result = repository.GetAll();

            // Assert
            Assert.AreEqual(2, result.Count());
            mockLogger.Verify(l => l.Log("All packages"), Times.Once);
        }
    }
}