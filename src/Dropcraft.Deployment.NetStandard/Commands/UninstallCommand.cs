using System;
using System.Linq;
using System.Threading.Tasks;
using Dropcraft.Common;
using Dropcraft.Common.Deployment;
using Microsoft.Extensions.CommandLineUtils;

namespace Dropcraft.Deployment.Commands
{
    /// <summary>
    /// Class UninstallCommand.
    /// </summary>
    /// <seealso cref="Dropcraft.Deployment.Commands.DeploymentCommand" />
    public class UninstallCommand : DeploymentCommand
    {
        private CommandArgument _package;
        private CommandOption _productPath;
        private CommandOption _removeDependencies;
        private CommandOption _enforce;

        /// <summary>
        /// Initializes a new instance of the <see cref="UninstallCommand"/> class.
        /// </summary>
        public UninstallCommand()
        {
            Name = "uninstall";
        }

        /// <summary>
        /// Defines the specified command application.
        /// </summary>
        /// <param name="cmdApp">The command application.</param>
        /// <param name="logErrorAction">The log error action.</param>
        protected override void Define(CommandLineApplication cmdApp, Action<string> logErrorAction)
        {
            cmdApp.Description = "Uninstalls provided packages from the product";
            cmdApp.HelpOption(CommandHelper.HelpOption);

            _package = cmdApp.Argument("[package]", "Package to install", true);
            _productPath = cmdApp.Option("--path <installationPath>", "Product installation path", CommandOptionType.SingleValue);

            _removeDependencies = cmdApp.Option("-r|--remove-deps", "Remove any dependent packages if they are not referenced elsewhere", CommandOptionType.NoValue);
            _enforce = cmdApp.Option("-e|--enforce", "Remove packages even if some other packages still depend on them", CommandOptionType.NoValue);
        }

        /// <summary>
        /// Executes the specified command application.
        /// </summary>
        /// <param name="cmdApp">The command application.</param>
        /// <param name="logErrorAction">The log error action.</param>
        /// <returns>Error Code</returns>
        protected override async Task<int> Execute(CommandLineApplication cmdApp, Action<string> logErrorAction)
        {
            if (_package.Values.Count == 0)
            {
                logErrorAction($"Missed packages list");
                return 1;
            }

            if (MissedOption(_productPath, logErrorAction))
                return 1;

            var engine = GetDeploymentEngine(cmdApp);
            var packageIds = _package.Values.Select(x => new PackageId(x)).ToArray();

            if (!_enforce.HasValue())
            {
                var packages = engine.DeploymentContext.ProductConfigurationProvider.GetPackages();
                var slice = packages.SliceWithDependents(packageIds);
                if (slice.Count != packageIds.Length)
                {
                    logErrorAction($"Some packages cannot be uninstalled because of the packages which depend on them");
                    return 1;
                }
            }

            var options = new UninstallationOptions {RemoveDependencies = _removeDependencies.HasValue()};
            await engine.UninstallPackages(packageIds, options);

            return await Task.FromResult(0);
        }

        /// <summary>
        /// Gets the deployment engine.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <returns><see cref="IDeploymentEngine"/></returns>
        protected virtual IDeploymentEngine GetDeploymentEngine(CommandLineApplication app)
        {
            var configuration = CommandHelper.GetConfiguration();
            return configuration.CreateEngine(_productPath.Value(), string.Empty);
        }
    }
}