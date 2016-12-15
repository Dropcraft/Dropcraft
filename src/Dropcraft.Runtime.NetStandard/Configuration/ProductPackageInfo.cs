using System.Collections.Generic;
using Dropcraft.Common.Configuration;

namespace Dropcraft.Runtime.Configuration
{
    public class ProductPackageInfo
    {
        public IPackageConfiguration Configuration { get; set; }
        public List<string> Files { get; } = new List<string>();
    }
}