using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using PackageManager.Models;
using PackageManager.Enums;

namespace PackageManager.Tests.Unit
{
    public class PackageTests
    {
        [Test]
        public void CompareTo_LowerVersion_ReturnsMinusOne()
        {
            var p1 = new Package("Test", new PackageVersion(1, 0, 0, VersionType.final));
            var p2 = new Package("Test", new PackageVersion(2, 0, 0, VersionType.final));

            var result = p1.CompareTo(p2);

            Assert.That(result, Is.LessThan(0));
        }

        [Test]
        public void Equals_SameNameAndVersion_ReturnsTrue()
        {
            var p1 = new Package("Test", new PackageVersion(1, 0, 0, VersionType.final));
            var p2 = new Package("Test", new PackageVersion(1, 0, 0, VersionType.final));

            Assert.That(p1.Equals(p2), Is.True);

        }
    }
}
