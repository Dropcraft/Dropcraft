using System;
using System.Threading.Tasks;
using Dropcraft.Common.Deployment;
using Microsoft.Extensions.CommandLineUtils;

namespace Dropcraft.Deployment.Commands
{
    /// <summary>
    /// InspectCommand provides information about the installed product
    /// </summary>
    public class InspectCommand : DeploymentCommand
    {
        private CommandOption _productPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="InspectCommand"/> class.
        /// </summary>
        public InspectCommand()
        {
            Name = "inspect";
        }

        /// <summary>
        /// Defines the specified command application.
        /// </summary>
        /// <param name="cmdApp">The command application.</param>
        /// <param name="logErrorAction">The log error action.</param>
        protected override void Define(CommandLineApplication cmdApp, Action<string> logErrorAction)
        {
            cmdApp.Description = "Provides information about the product";
            cmdApp.HelpOption(CommandHelper.HelpOption);

            _productPath = cmdApp.Option("--path <installationPath>", "Product installation path", CommandOptionType.SingleValue);
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="logErrorAction">The log error action.</param>
        /// <returns>Error code</returns>
        protected override async Task<int> Execute(CommandLineApplication app, Action<string> logErrorAction)
        {
            if (MissedOption(_productPath, logErrorAction))
                return 1;

            var configurationProvider = GetDeploymentEngine(app).DeploymentContext.ProductConfigurationProvider;
            if (!configurationProvider.IsProductConfigured)
            {
                logErrorAction($"Configuration not found at {_productPath}");
            }

            var packages = configurationProvider.GetPackages().FlattenLeastDependentFirst();
            foreach (var packageConfiguration in packages)
            {
                Console.WriteLine(packageConfiguration.ToString());
            }

            return await Task.FromResult(0);
        }

        /// <summary>
        /// Gets the deployment engine.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <returns><see cref="IDeploymentEngine"/></returns>
        protected virtual IDeploymentEngine GetDeploymentEngine(CommandLineApplication app)
        {
            return CommandHelper.GetConfiguration().CreateEngine(_productPath.Value(), string.Empty);
        }

    }
}