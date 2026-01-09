using NUnit.Framework;
using PackageManager.Models;
using PackageManager.Enums;

namespace PackageManager.Tests.Unit
{
    public class PackageVersionTests
    {
        [Test]
        public void Create_ValidVersion_NoException()
        {
            Assert.DoesNotThrow(() =>
            {
                var v = new PackageVersion(1, 0, 0, VersionType.final);

            });
        }

        [Test]
        public void CompareTo_SameVersion_ReturnsZero()
        {
            var v1 = new PackageVersion(1, 0, 0, VersionType.final);
            var v2 = new PackageVersion(1, 0, 0, VersionType.final);

            Assert.That(v1.CompareTo(v2), Is.EqualTo(0));
        }
    }
}
