using Dropcraft.Common.Package;

namespace Dropcraft.Common.Configuration
{
    public interface IRuntimePackageConfigParser
    {
        IRuntimeParsedPackageConfig Parse(PackageInfo packageInfo);
    }
}