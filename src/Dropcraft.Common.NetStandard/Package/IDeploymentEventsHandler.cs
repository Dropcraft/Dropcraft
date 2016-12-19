using Dropcraft.Common.Deployment;

namespace Dropcraft.Common.Package
{
    public interface IDeploymentEventsHandler
    {
        void RegisterEventHandlers(DeploymentContext context);
    }

}