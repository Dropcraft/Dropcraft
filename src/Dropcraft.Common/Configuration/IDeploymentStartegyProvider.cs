using System.Collections.Generic;

namespace Dropcraft.Common.Configuration
{
    public interface IDeploymentStartegyProvider
    {
        IEnumerable<PackageFileInfo> GetPackageFiles(PackageId packageId, string packagePath);
    }
}