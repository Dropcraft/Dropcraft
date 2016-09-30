using System;

namespace Dropcraft.Common.Configuration
{
    public class ExtensionInfo
    {
        public string Id { get; }

        public PackageInfo PackageInfo { get; }

        public string ClassName { get; }

        public string ExtensibilityPointId { get; }

        public ICustomConfiguration CustomConfiguration { get; private set; }


        public ExtensionInfo(PackageInfo packageInfo, string className, string extensibilityPointId,
                                        string id, ICustomConfiguration customConfiguration)
        {
            PackageInfo = packageInfo;
            ClassName = className;
            ExtensibilityPointId = extensibilityPointId;
            Id = id;
            CustomConfiguration = customConfiguration;
        }

        public override string ToString()
        {
            return $"{PackageInfo?.ToString() ?? ""}, {Id}, {ExtensibilityPointId}, {ClassName}";
        }

    }
}