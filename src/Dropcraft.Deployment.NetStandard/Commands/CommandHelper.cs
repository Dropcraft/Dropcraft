using System;

namespace Dropcraft.Deployment.Commands
{
    public class CommandHelper
    {
        public static string HelpOption = "--help";

        /// <summary>
        /// Returns configuration for the commands
        /// </summary>
        public static Func<string, string, string, DeploymentConfiguration> GetConfiguration =
            (installPath, packagesFolderPath, targetFramework) =>
                    new DeploymentConfiguration(installPath, packagesFolderPath, targetFramework);
    }
}