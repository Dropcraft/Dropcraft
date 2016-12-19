using System.Collections.Generic;
using Dropcraft.Common.Package;

namespace Dropcraft.Common.Deployment
{
    public interface IDeploymentStartegyProvider
    {
        IEnumerable<PackageFileInfo> GetPackageFiles(PackageId packageId, string packagePath);
    }
}