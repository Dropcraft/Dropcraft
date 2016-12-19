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
        public async Task PackageWithVersionCanBeResolved()
        {
            using (var helper = new TestDeploymentHelper().WithConfiguration().AndNuGetSource())
            {
                var engine = helper.CreatEngine();
                var result = await engine.NuGetEngine.ResolvePackages(new List<PackageId> {new PackageId("bootstrap", "[3.3.7]", false)});

                result.InnerNodes.Count.Should().Be(1);
                result.InnerNodes[0].Item.Key.Name.Should().Be("bootstrap");
                result.InnerNodes[0].Item.Key.Version.ToNormalizedString().Should().Be("3.3.7");
                result.InnerNodes[0].Key.TypeConstraint.Should().Be(LibraryDependencyTarget.Package);
            }
        }

        [Fact]
        public async Task PackageWithoutVersionCanBeResolved()
        {
            using (var helper = new TestDeploymentHelper().WithConfiguration().AndNuGetSource())
            {
                var engine = helper.CreatEngine();
                engine.NuGetEngine.UpdatePackages = true;
                var packageVersion = await engine.NuGetEngine.ResolvePackageVersion(new PackageId("bootstrap", "", false) );

                packageVersion.Version.Should().NotBeNullOrWhiteSpace();
            }
        }

        [Fact]
        public void CompatibleFrameworkShallBeResolved()
        {
            var fx = NuGetEngine.GetMostCompatibleFramework("net45",
                new[] {"net40", "net45", "sl5", "portable-net40+sl5+win8+wp8+wpa81"});
            fx.Should().Be("net45");
        }

        [Fact]
        public void PortableCompatibleFrameworkShallBeResolved()
        {
            var fx = NuGetEngine.GetMostCompatibleFramework("portable-net40+sl5+win8+wp8+wpa81",
                new[] {"net40", "net45", "sl5", "portable-net40+sl5+win8+wp8+wpa81"});
            fx.Should().Be("portable-net40+sl5+win8+wp8+wpa81");
        }

    }
}