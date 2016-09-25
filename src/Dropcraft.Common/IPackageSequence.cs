using System.Collections.Generic;

namespace Dropcraft.Common
{
    public interface IPackageSequence
    {
        IEnumerable<PackageInfo> GetPackages();
    }
}