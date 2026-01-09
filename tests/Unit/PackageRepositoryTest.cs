using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using PackageManager.Repositories;
using PackageManager.Models;
using PackageManager.Enums;
using PackageManager.Models.Contracts;
using Moq;
using PackageManager.Info.Contracts;
using System.Linq;

namespace PackageManager.Tests.Unit
{
    public class PackageRepositoryTests
    {
        [Test]
        public void Add_NewPackage_PackageExistsInRepository()
        {
            var logger = new Mock<ILogger>();
            var packages = new List<IPackage>();
            var repo = new PackageRepository(logger.Object, packages);
            var package = new Package("Test", new PackageVersion(1, 0, 0, VersionType.final));

            repo.Add(package);

            Assert.That(repo.GetAll().Count, Is.EqualTo(2));
        }

        [Test]
        public void Update_NewerVersion_ReplacesOld()
        {
            var logger = new Mock<ILogger>();
            var packages = new List<IPackage>();
            var repo = new PackageRepository(logger.Object, packages);
            var oldPackage = new Package("Test", new PackageVersion(1, 0, 0, VersionType.final));
            var newPackage = new Package("Test", new PackageVersion(2, 0, 0, VersionType.final));

            repo.Add(oldPackage);
            repo.Update(newPackage);

            var version = repo.GetAll().First().Version;

            Assert.That(version.Major, Is.EqualTo(2));
            Assert.That(version.Minor, Is.EqualTo(0));
            Assert.That(version.Patch, Is.EqualTo(0));

        }
    }
}
