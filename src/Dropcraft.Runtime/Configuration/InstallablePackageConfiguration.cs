using System;
using System.Collections.Generic;
using Dropcraft.Common;
using Dropcraft.Common.Configuration;

namespace Dropcraft.Runtime.Configuration
{
    internal class InstallablePackageConfiguration : IInstallablePackageConfiguration
    {
        public IEnumerable<PackageDeploymentHandlerInfo> GetPackageDeploymentHandlers()
        {
            throw new NotImplementedException();
        }

        public void Reconfigure(DeploymentContext deploymentContext)
        {
            throw new NotImplementedException();
        }

        public void AppendToApplicationConfiguration()
        {
            throw new NotImplementedException();
        }
    }
}
