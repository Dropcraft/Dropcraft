using System;
using System.Threading.Tasks;
using Microsoft.Extensions.CommandLineUtils;

namespace Dropcraft.Deployment.Commands
{
    /// <summary>
    /// Class RepairCommand.
    /// </summary>
    /// <seealso cref="Dropcraft.Deployment.Commands.DeploymentCommand" />
    public class RepairCommand : DeploymentCommand
    {
        private CommandOption _productPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="RepairCommand"/> class.
        /// </summary>
        public RepairCommand()
        {
            Name = "repair";
        }

        /// <summary>
        /// Defines the specified command application.
        /// </summary>
        /// <param name="cmdApp">The command application.</param>
        /// <param name="logErrorAction">The log error action.</param>
        protected override void Define(CommandLineApplication cmdApp, Action<string> logErrorAction)
        {
            cmdApp.Description = "Repairs the product by reinstalling all the packages";
            cmdApp.HelpOption(CommandHelper.HelpOption);

            _productPath = cmdApp.Option("--path <productPath>", "Path to the product to repair", CommandOptionType.SingleValue);
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="logErrorAction">The log error action.</param>
        /// <returns>Error code</returns>
        protected override Task<int> Execute(CommandLineApplication app, Action<string> logErrorAction)
        {
            return Task.FromResult(0);
        }
    }
}