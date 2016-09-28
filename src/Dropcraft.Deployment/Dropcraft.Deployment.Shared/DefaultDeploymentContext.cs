using Dropcraft.Common;
using Dropcraft.Common.Handler;

namespace Dropcraft.Deployment
{
    public class DefaultDeploymentContext : DeploymentContext
    {
        public DefaultDeploymentContext(string installPath, string packagesFolderPath) 
            : base(installPath, packagesFolderPath)
        {
        }

        protected override void OnRegisterDeploymentEventHandler(IDeploymentEventsHandler deploymentEventsHandler)
        {
            throw new System.NotImplementedException();
        }

        protected override void OnUnregisterDeploymentEventHandler(IDeploymentEventsHandler deploymentEventsHandler)
        {
            throw new System.NotImplementedException();
        }

        protected override void OnRaiseDeploymentEvent(DeploymentEvent deploymentEvent)
        {
            throw new System.NotImplementedException();
        }
    }
}