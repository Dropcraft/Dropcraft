using Dropcraft.Common.Package;

namespace Dropcraft.Common.Configuration
{
    public class RuntimeEventsHandlerInfo
    {
        public PackageInfo PackageInfo { get; }

        public string ClassName { get; }

        public RuntimeEventsHandlerInfo(PackageInfo packageInfo, string className)
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
