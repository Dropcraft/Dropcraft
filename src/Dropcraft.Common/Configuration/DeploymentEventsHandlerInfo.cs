namespace Dropcraft.Common.Configuration
{
    public class DeploymentEventsHandlerInfo
    {
        public PackageInfo PackageInfo { get; }

        public string ClassName { get; }

        public DeploymentEventsHandlerInfo(PackageInfo packageInfo, string className)
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