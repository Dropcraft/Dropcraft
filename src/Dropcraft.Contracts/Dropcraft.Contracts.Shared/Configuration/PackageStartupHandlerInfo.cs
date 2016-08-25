namespace Dropcraft.Contracts.Configuration
{
    public class PackageStartupHandlerInfo
    {
        public PackageInfo PackageInfo { get; }

        public string ClassName { get; }

        public PackageStartupHandlerInfo(PackageInfo packageInfo, string className)
        {
            PackageInfo = packageInfo;
            ClassName = className;
        }
    }
}
