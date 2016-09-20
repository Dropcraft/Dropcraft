using System.Collections.Generic;
using Dropcraft.Common.Configuration;
using Dropcraft.Common.Package;

namespace Dropcraft.Runtime.Configuration
{
    internal class ApplicationConfiguration : IApplicationConfiguration
    {
        public IEnumerable<PackageInfo> GetPackages()
        {
            throw new System.NotImplementedException();
        }
    }
}
