using System.Threading.Tasks;
using Dropcraft.Common;
using FluentAssertions;
using Xunit;

namespace Tests.Dropcraft.Deployment
{
    public class DeploymentEngineTests
    {
        [Fact]
        public async Task DeploymentEngine_shall_install_two_packages_with_exact_version()
        {
            using (var helper = new TestDeploymentHelper().WithDefaultEngine())
            {
                await helper.Engine.InstallPackages(new[] { new PackageId("bootstrap", "[3.3.7]", false) });
                helper.IsPackageExists("bootstrap.3.3.7").Should().BeTrue();
                helper.IsPackageExists("jQuery.1.9.1").Should().BeTrue();
            }
        }

        [Fact]
        public async Task DeploymentEngine_shall_install_one_package_with_last_version()
        {
            using (var helper = new TestDeploymentHelper().WithDefaultEngine())
            {
                await helper.Engine.InstallPackages(new[] { new PackageId("Newtonsoft.Json", string.Empty, false) });
                helper.IsSimilarPackageExists("Newtonsoft.Json").Should().BeTrue();
            }
        }
    }
}
