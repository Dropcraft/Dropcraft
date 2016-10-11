using System.Collections.Generic;
using System.Threading.Tasks;
using Dropcraft.Common;
using Dropcraft.Deployment.NuGet;
using FluentAssertions;
using Xunit;

namespace Dropcraft.Deployment.Workflow
{
    public class InstallationWorkflowTests
    {
        private async Task<InstallationContext> ExecuteResolveWorkflow(TestDeploymentHelper helper,
            IEnumerable<PackageId> newPackages, IEnumerable<PackageId> productPackages)
        {
            var engine = new NuGetEngine(helper.Configuration);

            var context = new InstallationContext(newPackages, productPackages);
            var workflow = new InstallationWorkflow(context, engine);

            await workflow.EnsureAllPackagesAreVersioned();
            await workflow.ResolvePackages();

            return context;
        }

        [Fact]
        public async Task Two_new_packages_resolved()
        {
            using (var helper = new TestDeploymentHelper().WithConfiguration().AndNuGetSource())
            {
                var newPackages = new[] {new PackageId("bootstrap", "[3.3.7]", false)};
                var productPackages = new PackageId[] {};
                var context = await ExecuteResolveWorkflow(helper, newPackages, productPackages);

                context.PackagesForInstallation.Count.Should().Be(2);
                context.PackagesForInstallation[0].Id.Id.Should().Be("jQuery");
                context.PackagesForInstallation[1].Id.Id.Should().Be("bootstrap");
            }
        }

        [Fact]
        public async Task Product_package_and_two_new_packages_resolved()
        {
            using (var helper = new TestDeploymentHelper().WithConfiguration().AndNuGetSource())
            {
                var newPackages = new[] { new PackageId("bootstrap", string.Empty, false) };
                var productPackages = new[] { new PackageId("Newtonsoft.Json", "9.0.1", false) };
                var context = await ExecuteResolveWorkflow(helper, newPackages, productPackages);

                context.PackagesForInstallation.Count.Should().Be(3);
            }
        }

        [Fact]
        public async Task Product_package_shall_be_updated()
        {
            using (var helper = new TestDeploymentHelper().WithConfiguration().AndNuGetSource())
            {
                var newPackages = new[] { new PackageId("bootstrap", "[3.3.7]", false) };
                var productPackages = new[] { new PackageId("bootstrap", "[3.2.0]", false) };
                var context = await ExecuteResolveWorkflow(helper, newPackages, productPackages);

                context.PackagesForInstallation.Count.Should().Be(2);
                context.PackagesForInstallation[0].Id.Id.Should().Be("jQuery");
                context.PackagesForInstallation[1].Id.Id.Should().Be("bootstrap");
                context.PackagesForInstallation[1].Id.VersionRange.Should().Be("3.3.7");

                context.ProductPackagesForDeletion.Count.Should().Be(1);
                context.ProductPackagesForDeletion[0].Id.Should().Be("bootstrap");
                context.ProductPackagesForDeletion[0].VersionRange.Should().Contain("3.2.0");
            }
        }

        [Fact]
        public async Task Product_package_shall_be_downgraded()
        {
            using (var helper = new TestDeploymentHelper().WithConfiguration().AndNuGetSource())
            {
                helper.Configuration.ConfigureTo.AllowDowngrades(true);

                var productPackages = new[] { new PackageId("bootstrap", "[3.3.7]", false) };
                var newPackages = new[] { new PackageId("bootstrap", "[3.2.0]", false) };
                var context = await ExecuteResolveWorkflow(helper, newPackages, productPackages);

                context.PackagesForInstallation.Count.Should().Be(2);
                context.PackagesForInstallation[0].Id.Id.Should().Be("jQuery");
                context.PackagesForInstallation[1].Id.Id.Should().Be("bootstrap");
                context.PackagesForInstallation[1].Id.VersionRange.Should().Be("3.2.0");

                context.ProductPackagesForDeletion.Count.Should().Be(1);
                context.ProductPackagesForDeletion[0].Id.Should().Be("bootstrap");
                context.ProductPackagesForDeletion[0].VersionRange.Should().Contain("3.3.7");
            }
        }

    }
}