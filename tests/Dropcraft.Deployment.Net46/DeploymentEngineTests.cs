using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dropcraft.Common;
using Dropcraft.Common.Deployment;
using Dropcraft.Runtime.Core;
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
                var options = new UninstallationOptions {RemoveDependencies = false};

                var engine = helper.CreatEngine();
                await engine.InstallPackages(
                        new[] {new PackageId("Newtonsoft.Json/8.0.3"), new PackageId("bootstrap/3.3.7")},
                        new InstallationOptions {AllowDowngrades = false, UpdatePackages = false});

                var source = new ProductConfigurationSource();
                var productConfig = source.GetProductConfigurationProvider(helper.InstallPath);
                var packageConfig = productConfig.GetPackageConfiguration(new PackageId("bootstrap/3.3.7"));
                var metadata = packageConfig.GetPackageMetadata();
                metadata.Title.Should().Be("Bootstrap CSS");

                var files = Directory.GetFiles(helper.InstallPath);
                files.Length.Should().Be(3);

                var dirs = Directory.GetDirectories(helper.InstallPath);
                dirs.Length.Should().Be(3);

                engine = helper.CreatEngine();
                await engine.InstallPackages(new[] {new PackageId("Newtonsoft.Json/9.0.1")},
                        new InstallationOptions {AllowDowngrades = false, UpdatePackages = true});

                engine = helper.CreatEngine();
                await engine.UninstallPackages(new[] {new PackageId("Newtonsoft.Json/9.0.1")}, options);

                files = Directory.GetFiles(helper.InstallPath);
                files.Length.Should().Be(1, "because Newtonsoft.Json is uninstalled");

                engine = helper.CreatEngine();
                await engine.UninstallPackages(new[] { new PackageId("bootstrap/3.3.7") }, options);

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

        [Fact]
        public async Task PackageWithDependencyAreResolvedInAdditionToProductPackage()
        {
            using (var helper = new TestDeploymentHelper().WithConfiguration().AndNuGetSource())
            {
                var newPackages = new[] { new PackageId("bootstrap", string.Empty, false) };
                var productPackages = new PackageGraphBuilder()
                    .Append(new PackageId("Newtonsoft.Json", "9.0.1", false), new PackageId[] {})
                    .Build();

                var engine = helper.CreatEngine();
                var discoveredPackages = await engine.PackageDiscoverer.Discover(productPackages, newPackages);
                var plan = await engine.PackageDeployer.PlanInstallation(productPackages, discoveredPackages);

                plan.InstallCount.Should().Be(2);
                var packages = plan.TargetPackageGraph.FlattenLeastDependentFirst();
                packages.FirstOrDefault(x => x.IsSamePackage(new PackageId("bootstrap/3.3.7"))).Should().NotBeNull();
                packages.FirstOrDefault(x => x.Id == "jQuery").Should().NotBeNull();

            }
        }

        [Fact]
        public async Task ProductPackageVersionIsUpdated()
        {
            using (var helper = new TestDeploymentHelper().WithConfiguration().AndNuGetSource())
            {
                var newPackages = new[] { new PackageId("bootstrap", "[3.3.7]", false) };
                var productPackages = new PackageGraphBuilder()
                    .Append(new PackageId("bootstrap", "3.2.0", false), new PackageId[] {})
                    .Build();

                var engine = helper.CreatEngine();
                var discoveredPackages = await engine.PackageDiscoverer.Discover(productPackages, newPackages);
                var plan = await engine.PackageDeployer.PlanInstallation(productPackages, discoveredPackages);

                plan.UpdateCount.Should().Be(1);
                plan.InstallCount.Should().Be(1);
                plan.Actions.Count.Should().Be(5);

                var packages = plan.TargetPackageGraph.FlattenLeastDependentFirst();
                packages.FirstOrDefault(x => x.IsSamePackage(new PackageId("bootstrap/3.3.7"))).Should().NotBeNull();
                packages.FirstOrDefault(x => x.Id == "jQuery").Should().NotBeNull();
            }
        }

        [Fact]
        public async Task ProductPackageVersionIsDowngraded()
        {
            using (var helper = new TestDeploymentHelper().WithConfiguration().AndNuGetSource())
            {
                var newPackages = new[] { new PackageId("bootstrap", "[3.2.0]", false) };
                var productPackages = new PackageGraphBuilder()
                    .Append(new PackageId("bootstrap", "3.7.0", false), new PackageId[] { })
                    .Build();

                var engine = helper.CreatEngine();
                engine.NuGetEngine.AllowDowngrades = true;
                var discoveredPackages = await engine.PackageDiscoverer.Discover(productPackages, newPackages);
                var plan = await engine.PackageDeployer.PlanInstallation(productPackages, discoveredPackages);

                var packages = plan.TargetPackageGraph.FlattenLeastDependentFirst();
                packages.FirstOrDefault(x => x.IsSamePackage(new PackageId("bootstrap/3.2.0"))).Should().NotBeNull();
                packages.FirstOrDefault(x => x.Id == "jQuery").Should().NotBeNull();
            }
        }

    }
}