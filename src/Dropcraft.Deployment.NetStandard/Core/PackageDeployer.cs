using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dropcraft.Common;
using Dropcraft.Common.Deployment;
using Dropcraft.Deployment.NuGet;
using Dropcraft.Runtime.Core;
using NuGet.DependencyResolver;
using NuGet.LibraryModel;

namespace Dropcraft.Deployment.Core
{
    /// <summary>
    /// Class PackageDeployer.
    /// </summary>
    /// <seealso cref="Dropcraft.Deployment.Core.IPackageDeployer" />
    public class PackageDeployer : IPackageDeployer
    {
        /// <summary>
        /// An instance of <see cref="INuGetEngine" /> to use
        /// </summary>
        public INuGetEngine NuGetEngine { get; set; }

        /// <summary>
        /// Packages installation path
        /// </summary>
        public string PackagesFolderPath { get; set; }

        /// <summary>
        /// An instance of <see cref="IDeploymentStrategyProvider" /> to use
        /// </summary>
        /// <value>The deployment strategy provider.</value>
        public IDeploymentStrategyProvider DeploymentStrategyProvider { get; set; }

        /// <summary>
        /// Current deployment context. An instance of <see cref="DeploymentContext" />
        /// </summary>
        /// <value>The deployment context.</value>
        public DeploymentContext DeploymentContext { get; set; }

        /// <summary>
        /// Generates an installation plan
        /// </summary>
        /// <param name="productPackages">Current product</param>
        /// <param name="packages">Packages to install</param>
        /// <returns><see cref="InstallationPlan" /></returns>
        public async Task<InstallationPlan> PlanInstallation(IPackageGraph productPackages, ICollection<PackageId> packages)
        {
            var plan = new InstallationPlan();
            var resolvedPackagesDictionary = new Dictionary<PackageId, DeploymentPackageInfo>();
            var resolvedPackages = await NuGetEngine.ResolvePackages(packages);
            NuGetEngine.AnalyzePackages(resolvedPackages);

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
                        DeploymentStrategyProvider));

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

        /// <summary>
        /// Generates an uninstallation plan
        /// </summary>
        /// <param name="productPackages">Current product</param>
        /// <param name="packages">Packages to uninstall</param>
        /// <param name="removeDependencies">Defines whether the package dependencies shall be removed as well. When false, only the requested packages will be deleted</param>
        /// <returns><see cref="UninstallationPlan" /></returns>
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

        /// <summary>
        /// Translates NuGet package graph into <see cref="PackageGraph"/> using <see cref="PackageGraphBuilder"/>
        /// </summary>
        /// <param name="node">NuGet package node</param>
        /// <param name="graphBuilder">The graph builder.</param>
        /// <param name="resolvedPackagesDictionary">The resolved packages dictionary.</param>
        /// <returns>PackageId for the translated package</returns>
        /// <exception cref="System.ArgumentException"></exception>
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