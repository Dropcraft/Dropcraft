using System.Collections.Generic;
using Dropcraft.Common.Package;

namespace Dropcraft.Runtime.Core
{
    public class ProductPackageInfo
    {
        public IPackageConfiguration Configuration { get; set; }
        public List<string> Files { get; set; } = new List<string>();
    }
}