using System.Collections.Generic;

namespace Dropcraft.Common.Configuration
{
    /// <summary>
    /// PackageConfiguration is an individual package configuration
    /// </summary>
    public abstract class PackageConfiguration
    {
        public bool IsPackageEnabled()
        {
            return OnIsPackageEnabled();
        }
        
        public EntityActivationMode GetPackageActivationMode()
        {
            return OnGetPackageActivationMode();
        }

        public IEnumerable<PackageStartupHandlerInfo> GetPackageStartupHandlers()
        {
            return OnGetPackageStartupHandlers();
        }

        public IEnumerable<RuntimeEventsHandlerInfo> GetRuntimeEventHandlers()
        {
            return OnGetRuntimeEventHandlers();
        }

        public IEnumerable<ExtensionInfo> GetExtensions()
        {
            return OnGetExtensions();
        }

        public IEnumerable<ExtensibilityPointInfo> GetExtensibilityPoints()
        {
            return OnGetExtensibilityPoints();
        }

        public IEnumerable<DeploymentEventsHandlerInfo> GetPackageDeploymentHandlers()
        {
            return OnGetPackageDeploymentHandlers();
        }

        protected abstract bool OnIsPackageEnabled();
        protected abstract EntityActivationMode OnGetPackageActivationMode();
        protected abstract IEnumerable<PackageStartupHandlerInfo> OnGetPackageStartupHandlers();
        protected abstract IEnumerable<RuntimeEventsHandlerInfo> OnGetRuntimeEventHandlers();
        protected abstract IEnumerable<ExtensionInfo> OnGetExtensions();
        protected abstract IEnumerable<ExtensibilityPointInfo> OnGetExtensibilityPoints();
        protected abstract IEnumerable<DeploymentEventsHandlerInfo> OnGetPackageDeploymentHandlers();
    }
}