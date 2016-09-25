using System.Collections.Generic;
using Dropcraft.Common;
using Dropcraft.Common.Configuration;

namespace Dropcraft.Runtime
{
    class DeferredContext
    {
        public List<PackageInfo> Packages { get; }

        public List<ExtensibilityPointInfo> ExtensibilityPoints { get; }

        public DeferredContext()
        {
            Packages = new List<PackageInfo>();
            ExtensibilityPoints = new List<ExtensibilityPointInfo>();
        }
    }
}