namespace Dropcraft.Common.Handlers
{
    public interface IDeploymentEventsHandler
    {
        void RegisterEventHandlers(DeploymentContext context);
    }

}