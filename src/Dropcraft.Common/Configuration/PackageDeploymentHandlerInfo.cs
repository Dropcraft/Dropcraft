using Dropcraft.Common.Package;

namespace Dropcraft.Common.Configuration
{
    public class PackageDeploymentHandlerInfo
    {
        public PackageInfo PackageInfo { get; }

        public string ClassName { get; }

        public PackageDeploymentHandlerInfo(PackageInfo packageInfo, string className)
        {
            PackageInfo = packageInfo;
            ClassName = className;
        }

        public override string ToString()
        {
            return $"{PackageInfo?.ToString() ?? ""}, {ClassName}";
        }
    }
}