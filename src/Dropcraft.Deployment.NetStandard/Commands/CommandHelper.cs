using System;

namespace Dropcraft.Deployment.Commands
{
    public class CommandHelper
    {
        public static string HelpOption = "--help";

        /// <summary>
        /// Returns configuration for the commands
        /// </summary>
        public static Func<DeploymentConfiguration> GetConfiguration = () => new DeploymentConfiguration();
    }
}