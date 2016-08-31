using System;

namespace Dropcraft.Common.Configuration
{
    public class ExtensionInfo
    {
        public string Id { get; }

        private readonly Func<Type, object> _configGetter;

        public PackageInfo PackageInfo { get; }

        public string ClassName { get; }

        public string ExtensibilityPointId { get; }

        public T GetConfiguration<T>() where T : class, new()
        {
            return (T)_configGetter(typeof(T));
        }

        public ExtensionInfo(PackageInfo packageInfo, string className, string extensibilityPointId,
                                        string id, Func<Type, object> configGetter)
        {
            PackageInfo = packageInfo;
            ClassName = className;
            ExtensibilityPointId = extensibilityPointId;
            Id = id;
            _configGetter = configGetter;
        }

        public override string ToString()
        {
            return $"{PackageInfo?.ToString() ?? ""}, {Id}, {ExtensibilityPointId}, {ClassName}";
        }

    }
}