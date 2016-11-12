namespace Dropcraft.Common.Configuration
{
    public interface IDeploymentStrategySource
    {
        IDeploymentStartegyProvider GetStartegyProvider(DeploymentContext deploymentContext);
    }
}