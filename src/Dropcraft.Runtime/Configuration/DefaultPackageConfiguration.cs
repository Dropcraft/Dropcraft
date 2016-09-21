using System.Collections.Generic;
using Dropcraft.Common.Configuration;

namespace Dropcraft.Runtime.Configuration
{
    public class DefaultPackageConfiguration : PackageConfiguration
    {
        protected override bool OnIsPackageEnabled()
        {
            throw new System.NotImplementedException();
        }

        protected override EntityActivationMode OnGetPackageActivationMode()
        {
            throw new System.NotImplementedException();
        }

        protected override IEnumerable<PackageStartupHandlerInfo> OnGetPackageStartupHandlers()
        {
            throw new System.NotImplementedException();
        }

        protected override IEnumerable<RuntimeEventsHandlerInfo> OnGetRuntimeEventHandlers()
        {
            throw new System.NotImplementedException();
        }

        protected override IEnumerable<ExtensionInfo> OnGetExtensions()
        {
            throw new System.NotImplementedException();
        }

        protected override IEnumerable<ExtensibilityPointInfo> OnGetExtensibilityPoints()
        {
            throw new System.NotImplementedException();
        }

        protected override IEnumerable<PackageDeploymentHandlerInfo> OnGetPackageDeploymentHandlers()
        {
            throw new System.NotImplementedException();
        }
    }
}