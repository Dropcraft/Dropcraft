namespace Dropcraft.Common.Deployment
{
    public interface IDeploymentStrategySource
    {
        IDeploymentStartegyProvider GetStartegyProvider(DeploymentContext deploymentContext);
    }
}