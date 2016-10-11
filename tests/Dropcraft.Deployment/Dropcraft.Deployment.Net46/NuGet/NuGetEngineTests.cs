using System.Collections.Generic;
using System.Threading.Tasks;
using Dropcraft.Common;
using FluentAssertions;
using NuGet.LibraryModel;
using Xunit;

namespace Dropcraft.Deployment.NuGet
{
    public class NuGetEngineTests
    {
        [Fact]
        public async Task Package_with_version_can_be_resolved()
        {
            using (var helper = new TestDeploymentHelper().WithConfiguration().AndNuGetSource())
            {
                var engine = new NuGetEngine(helper.Configuration);
                var result = await engine.ResolvePackages(new List<PackageId> {new PackageId("bootstrap", "[3.3.7]", false)});

                result.InnerNodes.Count.Should().Be(1);
                result.InnerNodes[0].Item.Key.Name.Should().Be("bootstrap");
                result.InnerNodes[0].Item.Key.Version.ToNormalizedString().Should().Be("3.3.7");
                result.InnerNodes[0].Key.TypeConstraint.Should().Be(LibraryDependencyTarget.Package);
            }
        }

        [Fact]
        public async Task Package_without_version_can_be_resolved()
        {
            using (var helper = new TestDeploymentHelper().WithConfiguration().AndNuGetSource())
            {
                helper.Configuration.ConfigureTo.UpdatePackagesFromSource(true);
                var engine = new NuGetEngine(helper.Configuration);
                var result = await engine.ResolveNuGetVersion(new PackageId("bootstrap", "", false) );

                result.Should().NotBeNullOrWhiteSpace();
            }
        }

    }
}