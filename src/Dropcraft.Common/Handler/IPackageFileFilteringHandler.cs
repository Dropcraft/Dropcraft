using System.Collections.Generic;

namespace Dropcraft.Common.Handler
{
    public interface IPackageFileFilteringHandler
    {
        void Filter(PackageInfo packageInfo, List<PackageFileInfo> files, DeploymentContext deploymentContext);
    }
}
