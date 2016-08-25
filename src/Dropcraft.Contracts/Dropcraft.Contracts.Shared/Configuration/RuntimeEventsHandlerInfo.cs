
namespace Dropcraft.Contracts.Configuration
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
    }
}
