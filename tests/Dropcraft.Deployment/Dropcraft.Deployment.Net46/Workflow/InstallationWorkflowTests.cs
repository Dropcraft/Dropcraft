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
        [Fact]
        public async Task Two_new_packages_resolved()
        {
            using (var helper = new TestDeploymentHelper().WithConfiguration().AndNuGetSource())
            {
                var newPackages = new[] {new PackageId("bootstrap", "[3.3.7]", false)};
                var productPackages = new PackageId[] {};

                var context = await TestContext.ExecuteResolveWorkflow(helper, newPackages, productPackages);

                context.Ctx.PackagesForInstallation.Count.Should().Be(2);
                context.Ctx.PackagesForInstallation[0].Id.Id.Should().Be("jQuery");
                context.Ctx.PackagesForInstallation[1].Id.Id.Should().Be("bootstrap");
            }
        }

        [Fact]
        public async Task Product_package_and_two_new_packages_resolved()
        {
            using (var helper = new TestDeploymentHelper().WithConfiguration().AndNuGetSource())
            {
                var newPackages = new[] { new PackageId("bootstrap", string.Empty, false) };
                var productPackages = new[] { new PackageId("Newtonsoft.Json", "9.0.1", false) };
                var context = await TestContext.ExecuteResolveWorkflow(helper, newPackages, productPackages);

                context.Ctx.PackagesForInstallation.Count.Should().Be(2);
            }
        }

        [Fact]
        public async Task Product_package_shall_be_updated()
        {
            using (var helper = new TestDeploymentHelper().WithConfiguration().AndNuGetSource())
            {
                var newPackages = new[] { new PackageId("bootstrap", "[3.3.7]", false) };
                var productPackages = new[] { new PackageId("bootstrap", "3.2.0", false) };
                var context = await TestContext.ExecuteResolveWorkflow(helper, newPackages, productPackages);

                context.Ctx.PackagesForInstallation.Count.Should().Be(2);
                context.Ctx.PackagesForInstallation[0].Id.Id.Should().Be("jQuery");
                context.Ctx.PackagesForInstallation[1].Id.Id.Should().Be("bootstrap");
                context.Ctx.PackagesForInstallation[1].Id.VersionRange.Should().Be("3.3.7");

                context.Ctx.ProductPackagesForDeletion.Count.Should().Be(1);
                context.Ctx.ProductPackagesForDeletion[0].Id.Should().Be("bootstrap");
                context.Ctx.ProductPackagesForDeletion[0].VersionRange.Should().Contain("3.2.0");
            }
        }

        [Fact]
        public async Task Product_package_shall_be_downgraded()
        {
            using (var helper = new TestDeploymentHelper().WithConfiguration().AndNuGetSource())
            {
                helper.Configuration.ConfigureTo.AllowDowngrades(true);

                var productPackages = new[] { new PackageId("bootstrap", "3.3.7", false) };
                var newPackages = new[] { new PackageId("bootstrap", "[3.2.0]", false) };
                var context = await TestContext.ExecuteResolveWorkflow(helper, newPackages, productPackages);

                context.Ctx.PackagesForInstallation.Count.Should().Be(2);
                context.Ctx.PackagesForInstallation[0].Id.Id.Should().Be("jQuery");
                context.Ctx.PackagesForInstallation[1].Id.Id.Should().Be("bootstrap");
                context.Ctx.PackagesForInstallation[1].Id.VersionRange.Should().Be("3.2.0");

                context.Ctx.ProductPackagesForDeletion.Count.Should().Be(1);
                context.Ctx.ProductPackagesForDeletion[0].Id.Should().Be("bootstrap");
                context.Ctx.ProductPackagesForDeletion[0].VersionRange.Should().Contain("3.3.7");
            }
        }

        [Fact]
        public async Task Two_packages_shall_be_installed()
        {
            using (var helper = new TestDeploymentHelper().WithConfiguration().AndNuGetSource())
            {
                var productPackages = new[] { new PackageId("Newtonsoft.Json", "9.0.1", false) };
                var newPackages = new[] { new PackageId("bootstrap", "[3.3.7]", false) };
                var context = await TestContext.ExecuteResolveWorkflow(helper, newPackages, productPackages);
                context.ExecuteInstall(helper.PackagesPath);

                helper.IsPackageExists("bootstrap", "3.3.7").Should().Be(true);
                helper.IsPackageExists("jQuery", "1.9.1").Should().Be(true);
                helper.IsPackageExists("Newtonsoft.Json", "9.0.1").Should().Be(false);
            }
        }

        class TestContext
        {
            public InstallationContext Ctx { get; set; }
            public InstallationWorkflow Workflow { get; set; }

            public static async Task<TestContext> ExecuteResolveWorkflow(TestDeploymentHelper helper,
                IEnumerable<PackageId> newPackages, IEnumerable<PackageId> productPackages)
            {
                var engine = new NuGetEngine(helper.Configuration);

                var context = new TestContext()
                {
                    Ctx = new InstallationContext(newPackages, productPackages),
                    Workflow = new InstallationWorkflow(engine)
                };

                await context.Workflow.EnsureAllPackagesAreVersioned(context.Ctx);
                await context.Workflow.ResolvePackages(context.Ctx);

                return context;
            }

            public TestContext ExecuteInstall(string path)
            {
                Workflow.InstallPackages(Ctx, path);
                return this;
            }

        }

    }
}