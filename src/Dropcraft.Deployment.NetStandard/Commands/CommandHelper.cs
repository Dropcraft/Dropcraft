using System;

namespace Dropcraft.Deployment.Commands
{
    /// <summary>
    /// CommandHelper allows to define a pre-configured DeploymentConfiguration to be used by the commands
    /// </summary>
    public class CommandHelper
    {
        public static string HelpOption = "--help";

        /// <summary>
        /// Returns configuration for the commands
        /// </summary>
        public static Func<DeploymentConfiguration> GetConfiguration = () => new DeploymentConfiguration();
    }
}