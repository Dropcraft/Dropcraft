using System.Collections.Generic;

namespace Dropcraft.Runtime.Configuration
{
    public class ProductPackageInfo
    {
        public List<string> Files { get; } = new List<string>();
        public List<string> Dependencies { get; } = new List<string>();
    }
}