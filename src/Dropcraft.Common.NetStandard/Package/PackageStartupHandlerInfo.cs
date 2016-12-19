namespace Dropcraft.Common.Package
{
    public class PackageStartupHandlerInfo
    {
        public PackageId PackageInfo { get; }

        public string ClassName { get; }

        public PackageStartupHandlerInfo(PackageId packageInfo, string className)
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
