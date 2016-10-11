using System.Collections.Generic;
using System.Threading.Tasks;
using Dropcraft.Common;
using FluentAssertions;
using Xunit;

namespace Dropcraft.Deployment.NuGet
{
    public class NuGetEngineTests
    {
        [Fact]
        public async Task Package_can_be_resolved_with_default_configuration()
        {
            using (var helper = new TestDeploymentHelper())
            {
                var configuration = new DeploymentConfiguration(helper.InstallPath, helper.PackagesPath);
                var engine = new NuGetEngine(configuration);

                var result = await engine.ResolvePackages(new List<PackageId> {new PackageId("bootstrap", "[3.3.7]", false)});

                result.InnerNodes.Count.Should().Be(1);
                result.InnerNodes[0].Item.Key.Name.Should().Be("bootstrap");
                result.InnerNodes[0].Item.Key.Version.ToNormalizedString().Should().Be("3.3.7");
            }
        }
    }
}