namespace Dropcraft.Common.Configuration
{
    public class ExtensibilityPointInfo
    {
        public PackageId PackageInfo { get; }

        public string ClassName { get; }

        public string Id { get; }

        public EntityActivationMode ActivationMode { get; }

        public ICustomConfiguration CustomConfiguration { get; private set; }

        public ExtensibilityPointInfo(PackageId packageInfo, string className, string id, 
                                        EntityActivationMode activationMode, ICustomConfiguration customConfiguration)
        {
            PackageInfo = packageInfo;
            ClassName = className;
            Id = id;
            ActivationMode = activationMode;
            CustomConfiguration = customConfiguration;
        }

        public override string ToString()
        {
            return $"{PackageInfo?.ToString() ?? ""}, {Id}, {ClassName}, {ActivationMode}";
        }
    }
}