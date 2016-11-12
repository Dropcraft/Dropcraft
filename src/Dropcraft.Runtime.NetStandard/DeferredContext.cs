using System.Collections.Generic;
using Dropcraft.Common.Configuration;

namespace Dropcraft.Runtime
{
    class DeferredContext
    {
        public List<IPackageConfiguration> PackageConfigurations { get; }

        public List<ExtensibilityPointInfo> ExtensibilityPoints { get; }

        public DeferredContext()
        {
            PackageConfigurations = new List<IPackageConfiguration>();
            ExtensibilityPoints = new List<ExtensibilityPointInfo>();
        }
    }
}