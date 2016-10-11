using System.Collections.Generic;
using Dropcraft.Common;
using Dropcraft.Common.Configuration;

namespace Dropcraft.Runtime
{
    class DeferredContext
    {
        public List<PackageId> Packages { get; }

        public List<ExtensibilityPointInfo> ExtensibilityPoints { get; }

        public DeferredContext()
        {
            Packages = new List<PackageId>();
            ExtensibilityPoints = new List<ExtensibilityPointInfo>();
        }
    }
}