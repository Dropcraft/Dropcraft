using Dropcraft.Common.Package;

namespace Dropcraft.Common.Configuration
{
    public interface IDeploymentPackageConfigParser
    {
        IDeploymentParsedPackageConfig Parse(PackageInfo packageInfo);
    }
}