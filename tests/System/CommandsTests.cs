using NUnit.Framework;

using Moq;
using PackageManager.Commands;
using PackageManager.Core.Contracts;
using PackageManager.Models;
using PackageManager.Models.Contracts;
using PackageManager.Enums;

namespace PackageManager.Tests.System
{
    public class CommandsTests
    {
        [Test]
        public void InstallCommand_Execute_CallsInstaller()
        {
            var installer = new Mock<IInstaller<IPackage>>();
            var package = new Package(
                "Test",
                new PackageVersion(1, 0, 0, VersionType.final)
            );

            var command = new InstallCommand(installer.Object, package);

            command.Execute();

            installer.Verify(i => i.PerformOperation(package), Times.Once);
        }
    }
}
