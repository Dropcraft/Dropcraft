namespace Dropcraft.Common.Package
{
    public class RuntimeEventsHandlerInfo
    {
        public PackageId PackageInfo { get; }

        public string ClassName { get; }

        public RuntimeEventsHandlerInfo(PackageId packageInfo, string className)
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
