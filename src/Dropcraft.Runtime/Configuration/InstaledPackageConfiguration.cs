using System;
using System.Collections.Generic;
using Dropcraft.Common.Configuration;

namespace Dropcraft.Runtime.Configuration
{
    internal class InstaledPackageConfiguration : IInstaledPackageConfiguration
    {
        public bool IsPackageEnabled()
        {
            throw new NotImplementedException();
        }

        public EntityActivationMode GetPackageActivationMode()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<PackageStartupHandlerInfo> GetPackageStartupHandlers()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<RuntimeEventsHandlerInfo> GetRuntimeEventHandlers()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ExtensionInfo> GetExtensions()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ExtensibilityPointInfo> GetExtensibilityPoints()
        {
            throw new NotImplementedException();
        }
    }
}
