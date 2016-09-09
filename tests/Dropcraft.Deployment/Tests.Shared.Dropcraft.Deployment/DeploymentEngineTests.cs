using System.Threading.Tasks;
using Dropcraft.Common.Package;
using FluentAssertions;
using Xunit;

namespace Tests.Dropcraft.Deployment
{
    public class DeploymentEngineTests
    {
        [Fact]
        public async Task DeploymentEngine_shall_install_one_package_with_exact_version()
        {
            using (var helper = new TestDeploymentHelper().WithDefaultEngine())
            {
                await helper.Engine.InstallPackages(new[] { new VersionedPackageInfo("Newtonsoft.Json", "9.0.1", false) });
                helper.IsPackageExists("Newtonsoft.Json.9.0.1").Should().BeTrue();
            }
        }

        [Fact]
        public async Task DeploymentEngine_shall_install_one_package_with_last_version()
        {
            using (var helper = new TestDeploymentHelper().WithDefaultEngine())
            {
                await helper.Engine.InstallPackages(new[] { new VersionedPackageInfo("Newtonsoft.Json", string.Empty, false) });
                helper.IsSimilarPackageExists("Newtonsoft.Json").Should().BeTrue();
            }
        }
    }
}
