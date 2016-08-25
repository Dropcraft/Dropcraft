using System;

namespace Dropcraft.Contracts.Configuration
{
    public class ExtensibilityPointInfo
    {
        private readonly Func<Type, object> _configGetter;

        public PackageInfo PackageInfo { get; }

        public string ClassName { get; }

        public string Id { get; }

        public EntityActivationMode ActivationMode { get; }

        public T GetConfiguration<T>() where T : class, new()
        {
            return (T)_configGetter(typeof(T));
        }

        public ExtensibilityPointInfo(PackageInfo packageInfo, string className, string id, 
                                        EntityActivationMode activationMode, Func<Type, object> configGetter)
        {
            PackageInfo = packageInfo;
            ClassName = className;
            Id = id;
            ActivationMode = activationMode;
            _configGetter = configGetter;
        }
    }
}