using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Dropcraft.Common;
using Dropcraft.Common.Deployment;
using Dropcraft.Common.Package;
using Dropcraft.Runtime.Core;

namespace Dropcraft.Deployment.Core
{
    /// <summary>
    /// Holds transactional information for the deployment session. Automatically rolls back the changes if not committed. 
    /// Rollback can be called manually using Dispose() method.
    /// </summary>
    public interface IDeploymentTransaction : IDisposable
    {
        /// <summary>
        /// Packages marked during the transaction as deleted using <see cref="TrackDeletedPackage"/>. 
        /// </summary>
        ReadOnlyCollection<PackageId> DeletedPackages { get; }

        /// <summary>
        /// Packages marked during the transaction as installed using <see cref="TrackInstalledPackage"/>. 
        /// </summary>
        ReadOnlyCollection<ProductPackageInfo> InstalledPackages { get; }

        /// <summary>
        /// Creates a new folder
        /// </summary>
        /// <param name="folder">Folder to create</param>
        void CreateFolder(string folder);

        /// <summary>
        /// Deletes file
        /// </summary>
        /// <param name="fileName">File to delete</param>
        void DeleteFile(IPackageFile fileName);

        /// <summary>
        /// Copies file according the defined rules 
        /// </summary>
        /// <param name="fileInfo">File to copy</param>
        void InstallFile(PackageFileDeploymentInfo fileInfo);

        /// <summary>
        /// Marks package as deleted. For information proposes only, rollback does not undelete the package.
        /// </summary>
        /// <param name="packageId">Deleted package</param>
        void TrackDeletedPackage(PackageId packageId);

        /// <summary>
        /// Marks package as installed. For information proposes only, rollback does not uninstall the package.
        /// </summary>
        /// <param name="packageConfiguration">Installed package</param>
        /// <param name="packageFiles">Installed package files</param>
        void TrackInstalledPackage(IPackageConfiguration packageConfiguration, IEnumerable<PackageFileInfo> packageFiles);

        /// <summary>
        /// Commits all the chages. 
        /// </summary>
        void Commit();
    }
}