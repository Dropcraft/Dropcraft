namespace Dropcraft.Common.Package
{
    public class DeploymentEventsHandlerInfo
    {
        public PackageId PackageInfo { get; }

        public string ClassName { get; }

        public DeploymentEventsHandlerInfo(PackageId packageInfo, string className)
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