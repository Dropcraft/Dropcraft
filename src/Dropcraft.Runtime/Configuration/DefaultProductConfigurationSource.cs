using System.Collections.Generic;
using Dropcraft.Common;
using Dropcraft.Common.Configuration;

namespace Dropcraft.Runtime.Configuration
{
    public class DefaultProductConfigurationSource : ProductConfigurationSource
    {
        /// <summary>
        /// Defines product configuration file name to store information about installed packages, dependencies, etc.
        /// </summary>
        public string ProductConfigurationFileName { get; set; } = "dropcraft.json";

        public DefaultProductConfigurationSource(ProductContext context)
            : base(context)
        {
        }

        protected override IEnumerable<PackageInfo> OnGetPackages()
        {
            throw new System.NotImplementedException();
        }

        protected override PackageConfiguration OnGetPackageConfiguration(PackageInfo packageInfo)
        {
            throw new System.NotImplementedException();
        }

        protected override void OnSetPackageConfiguration(PackageInfo packageInfo)
        {
            throw new System.NotImplementedException();
        }

        protected override bool OnIsProductConfigured()
        {
            throw new System.NotImplementedException();
        }
    }
}