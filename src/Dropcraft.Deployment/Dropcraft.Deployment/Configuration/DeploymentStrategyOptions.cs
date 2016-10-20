using Dropcraft.Common.Configuration;

namespace Dropcraft.Deployment.Configuration
{
    public class DeploymentStrategyOptions
    {
        readonly DeploymentConfiguration _configuration;

        public DeploymentStrategyOptions(DeploymentConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Allows to define custom deployment strategy source
        /// </summary>
        /// <param name="strategySource">Custom deployment strategy source</param>
        /// <returns>Configuration object</returns>
        public DeploymentConfiguration UseStrategy(IDeploymentStrategySource strategySource)
        {
            _configuration.DeploymentStrategySource = strategySource;
            return _configuration;
        }
    }
}