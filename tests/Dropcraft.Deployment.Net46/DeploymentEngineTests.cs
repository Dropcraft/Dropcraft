using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dropcraft.Common;
using Dropcraft.Runtime.Configuration;
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
                await engine.InstallPackages(new[] {new PackageId("Newtonsoft.Json/8.0.3"), new PackageId("bootstrap/3.3.7") }, false, false);

                var files = Directory.GetFiles(helper.InstallPath);
                files.Length.Should().Be(3);

                var dirs = Directory.GetDirectories(helper.InstallPath);
                dirs.Length.Should().Be(3);

                engine = helper.CreatEngine();
                await engine.InstallPackages(new[] { new PackageId("Newtonsoft.Json/9.0.1") }, false, true);

                engine = helper.CreatEngine();
                await engine.UninstallPackages(new[] {new PackageId("Newtonsoft.Json/9.0.1")}, false);

                files = Directory.GetFiles(helper.InstallPath);
                files.Length.Should().Be(1, "because Newtonsoft.Json is uninstalled");

                engine = helper.CreatEngine();
                await engine.UninstallPackages(new[] { new PackageId("bootstrap/3.3.7") }, false);

                dirs = Directory.GetDirectories(helper.InstallPath);
                dirs.Length.Should().Be(1);
            }
        }

        [Fact]
        public async Task PackageWithDependencyAreResolved()
        {
            using (var helper = new TestDeploymentHelper().WithConfiguration().AndNuGetSource())
            {
                var newPackages = new[] { new PackageId("bootstrap", "[3.3.7]", false) };
                var engine = helper.CreatEngine();
                var discoveredPackages = await engine.PackageDiscoverer.Discover(PackageGraph.Empty(), newPackages);
                var plan = await engine.PackageDeployer.PlanInstallation(PackageGraph.Empty(), discoveredPackages);

                plan.InstallCount.Should().Be(2);
                var packages = plan.TargetPackageGraph.FlattenLeastDependentFirst();
                packages.FirstOrDefault(x => x.IsSamePackage(new PackageId("bootstrap/3.3.7"))).Should().NotBeNull();
                packages.FirstOrDefault(x => x.Id == "jQuery").Should().NotBeNull();
            }
        }
    }
}