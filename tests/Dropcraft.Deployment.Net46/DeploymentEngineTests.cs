using System.IO;
using System.Threading.Tasks;
using Dropcraft.Common;
using FluentAssertions;
using Xunit;

namespace Dropcraft.Deployment
{
    public class DeploymentEngineTests
    {
        [Fact]
        public async Task IntegratedTest()
        {
            using (var helper = new TestDeploymentHelper().WithConfiguration().AndNuGetSource())
            {
                var engine = helper.CreatEngine();
                await engine.InstallPackages(new[] {new PackageId("Newtonsoft.Json/9.0.1"), new PackageId("bootstrap/3.3.7") });

                var files = Directory.GetFiles(helper.InstallPath);
                files.Length.Should().Be(3);

                var dirs = Directory.GetDirectories(helper.InstallPath);
                dirs.Length.Should().Be(3);

                engine = helper.CreatEngine();
                await engine.UninstallPackages(new[] {new PackageId("Newtonsoft.Json/9.0.1")});

                files = Directory.GetFiles(helper.InstallPath);
                files.Length.Should().Be(1, "because Newtonsoft.Json is uninstalled");

                engine = helper.CreatEngine();
                await engine.UninstallPackages(new[] { new PackageId("bootstrap/3.3.7") });

                dirs = Directory.GetDirectories(helper.InstallPath);
                dirs.Length.Should().Be(1);
            }

        }
    }
}