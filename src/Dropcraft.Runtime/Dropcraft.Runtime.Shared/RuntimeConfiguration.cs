using System;
using System.Collections.Generic;
using Dropcraft.Common;
using Dropcraft.Common.Configuration;

namespace Dropcraft.Runtime
{
    public class RuntimeConfiguration
    {
        private readonly List<PackageInfo> _packageSources;

        public IEnumerable<PackageInfo> PackageSources => _packageSources;

        public RuntimeContext RuntimeContext { get; set; }

        public IList<IRuntimePackageConfigParser> PackageConfigurationParsers { get; }

        public IDictionary<Type, Func<object>> ServiceFactories { get; }

        public RuntimeConfiguration()
        {
            _packageSources = new List<PackageInfo>();
            ServiceFactories = new Dictionary<Type, Func<object>>();
            PackageConfigurationParsers = new List<IRuntimePackageConfigParser>();
        }

        internal void AddPackageSources(IEnumerable<PackageInfo> packages)
        {
            _packageSources.AddRange(packages);
        }
    }
}

