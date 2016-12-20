using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dropcraft.Common;
using Dropcraft.Common.Deployment;
using Dropcraft.Deployment.NuGet;
using Dropcraft.Runtime.Configuration;
using Dropcraft.Runtime.Core;
using NuGet.DependencyResolver;
using NuGet.LibraryModel;

namespace Dropcraft.Deployment.Core
{
    public class PackageDeployer : IPackageDeployer
    {
        public INuGetEngine NuGetEngine { get; set; }
        public string PackagesFolderPath { get; set; }
        public IDeploymentStartegyProvider DeploymentStartegyProvider { get; set; }
        public DeploymentContext DeploymentContext { get; set; }

        public async Task<InstallationPlan> PlanInstallation(IPackageGraph productPackages, ICollection<PackageId> packages)
        {
            var plan = new InstallationPlan();
            var resolvedPackagesDictionary = new Dictionary<PackageId, DeploymentPackageInfo>();
            var resolvedPackages = await NuGetEngine.ResolvePackages(packages);
            NuGetEngine.AnalysePackages(resolvedPackages);

            var graphBuilder = new PackageGraphBuilder();
            foreach (var innerNode in resolvedPackages.InnerNodes)
            {
                TranslateNuGetGraphNode(innerNode, graphBuilder, resolvedPackagesDictionary);
            }

            plan.TargetPackageGraph = graphBuilder.Build();
            var resolvedPackagesList = plan.TargetPackageGraph.FlattenMostDependentFirst();
            var productPackagesList = productPackages.FlattenMostDependentFirst();

            var deploymentActions = new List<DeploymentAction>();

            foreach (var packageId in resolvedPackagesList)
            {
                var packageMatch = productPackagesList.FirstOrDefault(x => x.Id == packageId.Id);
                var isUpdate = (packageMatch != null) && (packageMatch.Version != packageId.Version);
                var install = (packageMatch == null) || isUpdate;

                if (isUpdate)
                {
                    deploymentActions.Add(new DeletePackageAction(DeploymentContext, true, packageMatch));
                    plan.UpdateCount++;
                }

                if (install)
                {
                    var packageInfo = resolvedPackagesDictionary[packageId];
                    deploymentActions.Add(new DownloadPackageAction(DeploymentContext, isUpdate,
                        packageInfo, NuGetEngine, PackagesFolderPath));
                    deploymentActions.Add(new InstallPackageAction(DeploymentContext, isUpdate, packageInfo,
                        DeploymentStartegyProvider));

                    if (!isUpdate)
                        plan.InstallCount++;
                }
                else
                {
                    plan.RemainingPackages.Add(packageId);
                }
            }

            plan.Actions = deploymentActions.Where(x => x is DownloadPackageAction)
                .Concat(deploymentActions.Where(x => x is DeletePackageAction))
                .Concat(deploymentActions.Where(x => x is InstallPackageAction))
                .ToList();

            return plan;
        }

        public async Task<UninstallationPlan> PlanUninstallation(IPackageGraph productPackages, ICollection<PackageId> packages,
            bool removeDependencies)
        {
            if (removeDependencies)
            {
                packages = productPackages.SliceWithDependencies(packages, true).FlattenMostDependentFirst();
            }

            var plan = new UninstallationPlan
            {
                Actions = packages.Select(package => new DeletePackageAction(DeploymentContext, false, package))
                    .Cast<DeploymentAction>()
                    .ToList()
            };

            return await Task.FromResult(plan);
        }

        protected PackageId TranslateNuGetGraphNode(GraphNode<RemoteResolveResult> node, PackageGraphBuilder graphBuilder,
            Dictionary<PackageId, DeploymentPackageInfo> resolvedPackagesDictionary)
        {
            if (node.Key.TypeConstraint != LibraryDependencyTarget.Package &&
                node.Key.TypeConstraint != LibraryDependencyTarget.PackageProjectExternal)
            {
                throw new ArgumentException($"Package {node.Key.Name} cannot be resolved from the sources");
            }

            var package = new DeploymentPackageInfo
            {
                Id = new PackageId(node.Item.Key.Name, node.Item.Key.Version.ToNormalizedString(),
                        node.Item.Key.Version.IsPrerelease),
                Match = node.Item.Data.Match
            };

            graphBuilder.Append(package.Id,
                node.InnerNodes.Select(x => TranslateNuGetGraphNode(x, graphBuilder, resolvedPackagesDictionary)));

            if (!resolvedPackagesDictionary.ContainsKey(package.Id))
                resolvedPackagesDictionary.Add(package.Id, package);

            return package.Id;
        }
    }
}