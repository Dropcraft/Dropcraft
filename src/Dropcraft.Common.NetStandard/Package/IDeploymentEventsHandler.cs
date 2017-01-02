using Dropcraft.Common.Deployment;

namespace Dropcraft.Common.Package
{
    /// <summary>
    /// Defines handler for the deployment events
    /// </summary>
    public interface IDeploymentEventsHandler
    {
        /// <summary>
        /// RegisterEventHandlers() is expected to register event handlers with DeploymentContext
        /// </summary>
        /// <param name="context">Current deployment context</param>
        void RegisterEventHandlers(DeploymentContext context);
    }

}