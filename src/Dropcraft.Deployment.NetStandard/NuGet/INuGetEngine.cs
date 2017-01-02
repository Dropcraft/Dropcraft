using System.Collections.Generic;
using System.Threading.Tasks;
using Dropcraft.Common;
using NuGet.DependencyResolver;
using NuGet.Versioning;

namespace Dropcraft.Deployment.NuGet
{
    /// <summary>
    /// Interface INuGetEngine. Abstracts low-level NuGet operations
    /// </summary>
    public interface INuGetEngine
    {
        /// <summary>
        /// Gets or sets a value indicating whether package downgrades are allowed.
        /// </summary>
        bool AllowDowngrades { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether packages shall always be updated from the remote source.
        /// </summary>
        /// <value>When false, remote package sources will only be used when the packages cannot be resolved from the local sources</value>
        bool UpdatePackages { get; set; }

        /// <summary>
        /// Analyses the packages graph for potential issues (downgrades, missed package, circular dependency, etc.)
        /// </summary>
        /// <param name="resolvedPackages">The resolved packages.</param>
        void AnalyzePackages(GraphNode<RemoteResolveResult> resolvedPackages);

        /// <summary>
        /// Gets the package target path for installation.
        /// </summary>
        /// <param name="packageId">The package identifier.</param>
        /// <param name="version">Package NuGet version</param>
        /// <param name="path">Packages installation path</param>
        /// <returns>Appropriate package installation path, in a form of ...\[packages-path]\[package-id]\[package-version]\ </returns>
        string GetPackageTargetPath(string packageId, NuGetVersion version, string path);

        /// <summary>
        /// Installs the package.
        /// </summary>
        /// <param name="match">NuGet package match.</param>
        /// <param name="path">Installation path</param>
        /// <returns>Task</returns>
        /// <seealso cref="INuGetEngine.GetPackageTargetPath"/>
        Task InstallPackage(RemoteMatch match, string path);

        /// <summary>
        /// Resolves the packages and dependencies
        /// </summary>
        /// <param name="packages">The packages.</param>
        /// <returns>NuGet graph of the resolved packages</returns>
        Task<GraphNode<RemoteResolveResult>> ResolvePackages(ICollection<PackageId> packages);

        /// <summary>
        /// Resolves the package version.
        /// </summary>
        /// <param name="packageId">The package identifier.</param>
        /// <returns><see cref="PackageId"/></returns>
        Task<PackageId> ResolvePackageVersion(PackageId packageId);
    }
}