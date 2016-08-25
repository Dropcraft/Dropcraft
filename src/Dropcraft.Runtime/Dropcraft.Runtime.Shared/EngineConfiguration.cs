using System;
using System.Collections.Generic;
using Dropcraft.Contracts;
using Dropcraft.Contracts.Configuration;

namespace Dropcraft.Runtime
{
    public class EngineConfiguration
    {
        private readonly List<PackageInfo> _packageSources;

        public IEnumerable<PackageInfo> PackageSources => _packageSources;

        public RuntimeContext RuntimeContext { get; set; }

        public IList<IPackageConfigurationParser> PackageConfigurationParsers { get; }

        public IDictionary<Type, Func<object>> ServiceFactories { get; }

        public string DropInPath { get; set; }

        public TimeSpan DropInWatchPeriod { get; set; }

        public EngineConfiguration()
        {
            _packageSources = new List<PackageInfo>();
            ServiceFactories = new Dictionary<Type, Func<object>>();
            PackageConfigurationParsers = new List<IPackageConfigurationParser>();
        }

        internal void AddPackageSources(IEnumerable<PackageInfo> packages)
        {
            _packageSources.AddRange(packages);
        }
    }
}

