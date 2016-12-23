using System.Collections.Generic;
using Dropcraft.Common.Package;

namespace Dropcraft.Runtime.Core
{
    public class ProductPackageInfo
    {
        public IPackageConfiguration Configuration { get; set; }
        public List<IPackageFile> Files { get; set; } = new List<IPackageFile>();
    }
}