using System;
using System.Collections.Generic;
using Dropcraft.Common;
using Dropcraft.Common.Deployment;
using Dropcraft.Common.Package;
using Dropcraft.Runtime.Configuration;
using Dropcraft.Runtime.Core;

namespace Dropcraft.Deployment.Core
{
    public interface IDeploymentTransaction : IDisposable
    {
        List<PackageId> DeletedPackages { get; }
        List<ProductPackageInfo> InstalledPackages { get; }

        void CreateFolder(string folder);
        void DeleteFile(string fileName);
        void InstallFile(PackageFileInfo fileInfo);
        void TrackDeletedPackage(PackageId packageId);
        void TrackInstalledPackage(IPackageConfiguration packageConfiguration, IEnumerable<string> packageFiles);
        void Commit();
    }
}