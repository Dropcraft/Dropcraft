using System.Collections.Generic;

namespace Dropcraft.Common.Deployment
{
    /// <summary>
    /// File handling strategy for package deployment. Identifies files to install and target path, resolves conflicts. 
    /// </summary>
    public interface IDeploymentStrategyProvider
    {
        /// <summary>
        /// Returns files required for installation for the provided package
        /// </summary>
        /// <param name="packageId">Package been installed</param>
        /// <param name="packagePath">Path to the unpacked package</param>
        /// <returns>List of the files</returns>
        IReadOnlyCollection<PackageFileDeploymentInfo> GetPackageFiles(PackageId packageId, string packagePath);
    }
}