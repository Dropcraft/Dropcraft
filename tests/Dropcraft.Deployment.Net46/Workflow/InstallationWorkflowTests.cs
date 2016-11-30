using System.Collections.Generic;
using System.Threading.Tasks;
using Dropcraft.Common;
using FluentAssertions;
using Xunit;

namespace Dropcraft.Deployment.Workflow
{
    public class InstallationWorkflowTests
    {
        [Fact]
        public async Task PackageWithDependencyAreResolved()
        {
            using (var helper = new TestDeploymentHelper().WithConfiguration().AndNuGetSource())
            {
                var newPackages = new[] {new PackageId("bootstrap", "[3.3.7]", false)};
                var productPackages = new PackageId[] {};
                var topLevelProductPackages = new PackageId[] {};

                var context = await
                        TestContext.ExecuteResolveWorkflow(helper, newPackages, productPackages, topLevelProductPackages);

                context.Ctx.PackagesForInstallation.Count.Should().Be(2);
                context.Ctx.PackagesForInstallation[0].Id.Id.Should().Be("jQuery");
                context.Ctx.PackagesForInstallation[1].Id.Id.Should().Be("bootstrap");
            }
        }

        [Fact]
        public async Task PackageWithDependencyAreResolvedInAdditionToProductPackage()
        {
            using (var helper = new TestDeploymentHelper().WithConfiguration().AndNuGetSource())
            {
                var newPackages = new[] { new PackageId("bootstrap", string.Empty, false) };
                var productPackages = new[] { new PackageId("Newtonsoft.Json", "9.0.1", false) };
                var topLevelProductPackages = new[] { new PackageId("Newtonsoft.Json", "9.0.1", false) };

                var context = await TestContext.ExecuteResolveWorkflow(helper, newPackages, productPackages, topLevelProductPackages);

                context.Ctx.PackagesForInstallation.Count.Should().Be(2);
            }
        }

        [Fact]
        public async Task ProductPackageVersionIsUpdated()
        {
            using (var helper = new TestDeploymentHelper().WithConfiguration().AndNuGetSource())
            {
                var newPackages = new[] { new PackageId("bootstrap", "[3.3.7]", false) };
                var productPackages = new[] { new PackageId("bootstrap", "3.2.0", false) };
                var topLevelProductPackages = new[] { new PackageId("bootstrap", "3.2.0", false) };

                var context = await TestContext.ExecuteResolveWorkflow(helper, newPackages, productPackages, topLevelProductPackages);

                context.Ctx.PackagesForInstallation.Count.Should().Be(2);
                context.Ctx.PackagesForInstallation[0].Id.Id.Should().Be("jQuery");
                context.Ctx.PackagesForInstallation[1].Id.Id.Should().Be("bootstrap");
                context.Ctx.PackagesForInstallation[1].Id.Version.Should().Be("3.3.7");

                context.Ctx.ProductPackagesForDeletion.Count.Should().Be(1);
                context.Ctx.ProductPackagesForDeletion[0].Id.Should().Be("bootstrap");
                context.Ctx.ProductPackagesForDeletion[0].Version.Should().Contain("3.2.0");
            }
        }

        [Fact]
        public async Task ProductPackageVersionIsDowngraded()
        {
            using (var helper = new TestDeploymentHelper().WithConfiguration().AndNuGetSource())
            {
                var productPackages = new[] { new PackageId("bootstrap", "3.3.7", false) };
                var newPackages = new[] { new PackageId("bootstrap", "[3.2.0]", false) };
                var topLevelProductPackages = new[] { new PackageId("bootstrap", "3.3.7", false) };

                var context = await TestContext.ExecuteResolveWorkflow(helper, newPackages, productPackages, topLevelProductPackages, true);

                context.Ctx.PackagesForInstallation.Count.Should().Be(2);
                context.Ctx.PackagesForInstallation[0].Id.Id.Should().Be("jQuery");
                context.Ctx.PackagesForInstallation[1].Id.Id.Should().Be("bootstrap");
                context.Ctx.PackagesForInstallation[1].Id.Version.Should().Be("3.2.0");

                context.Ctx.ProductPackagesForDeletion.Count.Should().Be(1);
                context.Ctx.ProductPackagesForDeletion[0].Id.Should().Be("bootstrap");
                context.Ctx.ProductPackagesForDeletion[0].Version.Should().Contain("3.3.7");
            }
        }

        [Fact]
        public async Task PackageWithDependencyAreInstalledInAdditionToProductPackage()
        {
            using (var helper = new TestDeploymentHelper().WithConfiguration().AndNuGetSource())
            {
                var productPackages = new[] { new PackageId("Newtonsoft.Json", "9.0.1", false) };
                var newPackages = new[] { new PackageId("bootstrap", "[3.3.7]", false) };
                var topLevelProductPackages = new[] { new PackageId("Newtonsoft.Json", "9.0.1", false) };

                var context = await TestContext.ExecuteResolveWorkflow(helper, newPackages, productPackages, topLevelProductPackages);
                context.ExecuteInstall(helper.PackagesPath);

                helper.IsPackageExists("bootstrap", "3.3.7").Should().Be(true);
                helper.IsPackageExists("jQuery", "1.9.1").Should().Be(true);
                helper.IsPackageExists("Newtonsoft.Json", "9.0.1").Should().Be(false);
            }
        }

        class TestContext
        {
            public WorkflowContext Ctx { get; private set; }
            private DeploymentWorkflow Workflow { get; set; }

            public static async Task<TestContext> ExecuteResolveWorkflow(TestDeploymentHelper helper,
                IEnumerable<PackageId> newPackages, IEnumerable<PackageId> productPackages,
                IEnumerable<PackageId> topLevelProductPackages, bool allowDowngrades = false)
            {
                var deploymentEngine = helper.CreatEngine();
                var engine = helper.CreatNuGetEngine();
                engine.AllowDowngrades = allowDowngrades;

                var workflowContext = new WorkflowContext(newPackages, productPackages, topLevelProductPackages);
                var context = new TestContext
                {
                    Ctx = workflowContext,
                    Workflow = new DeploymentWorkflow(deploymentEngine.DeploymentContext, workflowContext, engine)
                };

                await context.Workflow.EnsureAllPackagesAreVersioned();
                await context.Workflow.ResolvePackages();

                return context;
            }

            public TestContext ExecuteInstall(string path)
            {
                Workflow.DownloadPackages(path);
                return this;
            }

        }

    }
}