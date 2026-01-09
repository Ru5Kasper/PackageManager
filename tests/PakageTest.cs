using NUnit.Framework;
using PackageManager.Models;
using PackageManager.Models.Contracts;
using PackageManager.Enums;
using System;
using System.Collections.Generic;
using Moq;

namespace PackageManager.Tests.Models
{
    [TestFixture]
    public class PackageTests
    {
        private IVersion mockVersion;
        private IPackage mockPackage;
        private ICollection<IPackage> dependencies;

        [SetUp]
        public void SetUp()
        {
            mockVersion = new Mock<IVersion>().Object;
            mockPackage = new Mock<IPackage>().Object;
            dependencies = new List<IPackage>();
        }

        [Test]
        public void Constructor_ShouldThrowArgumentNullException_WhenNameIsNull()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new Package(null, mockVersion));
        }

        [Test]
        public void Constructor_ShouldThrowArgumentNullException_WhenVersionIsNull()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new Package("TestPackage", null));
        }

        [Test]
        public void Constructor_ShouldInitializeDependencies_WhenDependenciesIsNull()
        {
            // Arrange
            var mockVersion = new Mock<IVersion>();
            mockVersion.Setup(v => v.Major).Returns(1);
            mockVersion.Setup(v => v.Minor).Returns(0);
            mockVersion.Setup(v => v.Patch).Returns(0);
            mockVersion.Setup(v => v.VersionType).Returns(VersionType.alpha);

            // Act
            var package = new Package("TestPackage", mockVersion.Object, null);

            // Assert
            Assert.IsNotNull(package.Dependencies);
            Assert.AreEqual(0, package.Dependencies.Count);
        }

        [Test]
        public void Constructor_ShouldSetCorrectUrl()
        {
            // Arrange
            var mockVersion = new Mock<IVersion>();
            mockVersion.Setup(v => v.Major).Returns(1);
            mockVersion.Setup(v => v.Minor).Returns(2);
            mockVersion.Setup(v => v.Patch).Returns(3);
            mockVersion.Setup(v => v.VersionType).Returns(VersionType.beta);

            // Act
            var package = new Package("TestPackage", mockVersion.Object);

            // Assert
            Assert.AreEqual("1.2.3-beta", package.Url);
        }

        [Test]
        public void CompareTo_ShouldThrowArgumentNullException_WhenOtherIsNull()
        {
            // Arrange
            var mockVersion = new Mock<IVersion>();
            var package = new Package("TestPackage", mockVersion.Object);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                package.CompareTo(null));
        }

        [Test]
        public void CompareTo_ShouldThrowArgumentException_WhenNamesAreDifferent()
        {
            // Arrange
            var mockVersion1 = new Mock<IVersion>();
            var mockVersion2 = new Mock<IVersion>();
            var mockPackage = new Mock<IPackage>();
            
            mockPackage.Setup(p => p.Name).Returns("DifferentPackage");
            mockPackage.Setup(p => p.Version).Returns(mockVersion2.Object);
            
            var package = new Package("TestPackage", mockVersion1.Object);

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                package.CompareTo(mockPackage.Object));
        }

        [Test]
        public void CompareTo_ShouldReturn1_WhenThisVersionIsGreater()
        {
            // Arrange
            var mockVersion1 = new Mock<IVersion>();
            mockVersion1.Setup(v => v.Major).Returns(2);
            mockVersion1.Setup(v => v.Minor).Returns(0);
            mockVersion1.Setup(v => v.Patch).Returns(0);
            mockVersion1.Setup(v => v.VersionType).Returns(VersionType.alpha);

            var mockVersion2 = new Mock<IVersion>();
            mockVersion2.Setup(v => v.Major).Returns(1);
            mockVersion2.Setup(v => v.Minor).Returns(0);
            mockVersion2.Setup(v => v.Patch).Returns(0);
            mockVersion2.Setup(v => v.VersionType).Returns(VersionType.alpha);

            var mockPackage = new Mock<IPackage>();
            mockPackage.Setup(p => p.Name).Returns("TestPackage");
            mockPackage.Setup(p => p.Version).Returns(mockVersion2.Object);

            var package = new Package("TestPackage", mockVersion1.Object);

            // Act
            var result = package.CompareTo(mockPackage.Object);

            // Assert
            Assert.AreEqual(1, result);
        }

        [Test]
        public void CompareTo_ShouldReturnMinus1_WhenThisVersionIsLower()
        {
            // Arrange
            var mockVersion1 = new Mock<IVersion>();
            mockVersion1.Setup(v => v.Major).Returns(1);
            mockVersion1.Setup(v => v.Minor).Returns(0);
            mockVersion1.Setup(v => v.Patch).Returns(0);
            mockVersion1.Setup(v => v.VersionType).Returns(VersionType.alpha);

            var mockVersion2 = new Mock<IVersion>();
            mockVersion2.Setup(v => v.Major).Returns(2);
            mockVersion2.Setup(v => v.Minor).Returns(0);
            mockVersion2.Setup(v => v.Patch).Returns(0);
            mockVersion2.Setup(v => v.VersionType).Returns(VersionType.alpha);

            var mockPackage = new Mock<IPackage>();
            mockPackage.Setup(p => p.Name).Returns("TestPackage");
            mockPackage.Setup(p => p.Version).Returns(mockVersion2.Object);

            var package = new Package("TestPackage", mockVersion1.Object);

            // Act
            var result = package.CompareTo(mockPackage.Object);

            // Assert
            Assert.AreEqual(-1, result);
        }

        [Test]
        public void Equals_ShouldThrowArgumentNullException_WhenObjIsNull()
        {
            // Arrange
            var mockVersion = new Mock<IVersion>();
            var package = new Package("TestPackage", mockVersion.Object);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                package.Equals(null));
        }

        [Test]
        public void Equals_ShouldThrowArgumentException_WhenObjIsNotIPackage()
        {
            // Arrange
            var mockVersion = new Mock<IVersion>();
            var package = new Package("TestPackage", mockVersion.Object);
            var notPackage = new object();

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                package.Equals(notPackage));
        }

        [Test]
        public void Equals_ShouldReturnTrue_WhenPackagesAreEqual()
        {
            // Arrange
            var mockVersion = new Mock<IVersion>();
            mockVersion.Setup(v => v.Major).Returns(1);
            mockVersion.Setup(v => v.Minor).Returns(2);
            mockVersion.Setup(v => v.Patch).Returns(3);
            mockVersion.Setup(v => v.VersionType).Returns(VersionType.alpha);

            var mockPackage = new Mock<IPackage>();
            mockPackage.Setup(p => p.Name).Returns("TestPackage");
            mockPackage.Setup(p => p.Version).Returns(mockVersion.Object);

            var package = new Package("TestPackage", mockVersion.Object);

            // Act
            var result = package.Equals(mockPackage.Object);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void Equals_ShouldReturnFalse_WhenPackagesAreDifferent()
        {
            // Arrange
            var mockVersion1 = new Mock<IVersion>();
            mockVersion1.Setup(v => v.Major).Returns(1);
            mockVersion1.Setup(v => v.Minor).Returns(2);
            mockVersion1.Setup(v => v.Patch).Returns(3);
            mockVersion1.Setup(v => v.VersionType).Returns(VersionType.alpha);

            var mockVersion2 = new Mock<IVersion>();
            mockVersion2.Setup(v => v.Major).Returns(2);
            mockVersion2.Setup(v => v.Minor).Returns(0);
            mockVersion2.Setup(v => v.Patch).Returns(0);
            mockVersion2.Setup(v => v.VersionType).Returns(VersionType.alpha);

            var mockPackage = new Mock<IPackage>();
            mockPackage.Setup(p => p.Name).Returns("TestPackage");
            mockPackage.Setup(p => p.Version).Returns(mockVersion2.Object);

            var package = new Package("TestPackage", mockVersion1.Object);

            // Act
            var result = package.Equals(mockPackage.Object);

            // Assert
            Assert.IsFalse(result);
        }
    }
}