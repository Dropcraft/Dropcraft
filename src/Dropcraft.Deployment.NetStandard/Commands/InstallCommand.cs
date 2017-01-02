using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dropcraft.Common;
using Dropcraft.Common.Deployment;
using Microsoft.Extensions.CommandLineUtils;

namespace Dropcraft.Deployment.Commands
{
    /// <summary>
    /// Class InstallCommand.
    /// </summary>
    /// <seealso cref="Dropcraft.Deployment.Commands.DeploymentCommand" />
    public class InstallCommand : DeploymentCommand
    {
        private CommandArgument _package;
        private CommandOption _productPath;
        private CommandOption _packagePath;
        private CommandOption _framework;
        private CommandOption _source;
        private CommandOption _localSource;
        private CommandOption _updatePackages;
        private CommandOption _allowDowngrades;

        /// <summary>
        /// Initializes a new instance of the <see cref="InstallCommand"/> class.
        /// </summary>
        public InstallCommand()
        {
            Name = "install";
        }

        /// <summary>
        /// Defines the specified command application.
        /// </summary>
        /// <param name="cmdApp">The command application.</param>
        /// <param name="logErrorAction">The log error action.</param>
        protected override void Define(CommandLineApplication cmdApp, Action<string> logErrorAction)
        {
            cmdApp.Description = "Installs provided packages to the target path and updates the product configuration";
            cmdApp.HelpOption(CommandHelper.HelpOption);

            _package = cmdApp.Argument("[package]", "Package to install", true);

            _productPath = cmdApp.Option("--path <installationPath>", "Product installation path", CommandOptionType.SingleValue);
            _packagePath = cmdApp.Option("--packages <packagesDirectory>", "Packages installation path, used as a local packages cache", CommandOptionType.SingleValue);
            _framework = cmdApp.Option("--framework <frameworkId>", "Target framework ID", CommandOptionType.SingleValue);
            _source = cmdApp.Option("-s|--source <packageSource>", "Remote package source, URL or path", CommandOptionType.MultipleValue);
            _localSource = cmdApp.Option("-l|--local <packageSource>", "Local package source, URL or path", CommandOptionType.MultipleValue);

            _updatePackages = cmdApp.Option("--update-packages", "Always try to update packages from the remote sources", CommandOptionType.NoValue);
            _allowDowngrades = cmdApp.Option("--allow-downgrades", "Allow packages downgrades", CommandOptionType.NoValue);
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

            if (MissedOption(_framework, logErrorAction))
                return 1;

            if (MissedOption(_source, logErrorAction))
                return 1;

            var packageIds = new List<PackageId>();
            foreach (var packageValue in _package.Values)
            {
                var parts = packageValue.Split('/');
                if (parts.Length == 1)
                {
                    packageIds.Add(new PackageId(parts[0], string.Empty, false));
                }
                else
                {
                    var allowPrerelease = parts[1].Contains("-") ||
                                          (parts.Length == 3 && parts[2].Contains("allow-prerelease"));

                    packageIds.Add(new PackageId(parts[0], parts[1], allowPrerelease));
                }
            }

            var engine = GetDeploymentEngine(cmdApp);
            var options = new InstallationOptions
            {
                AllowDowngrades = _allowDowngrades.HasValue(),
                UpdatePackages = _updatePackages.HasValue()
            };

            await engine.InstallPackages(packageIds, options);

            return 0;
        }

        /// <summary>
        /// Gets the deployment engine.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <returns><see cref="IDeploymentEngine"/></returns>
        protected virtual IDeploymentEngine GetDeploymentEngine(CommandLineApplication app)
        {
            var configuration = CommandHelper.GetConfiguration()
                .ForPackages.Cache(_packagePath.HasValue() ? _packagePath.Value() : string.Empty);

            foreach (var sourceValue in _source.Values)
            {
                configuration.ForPackages.AddRemoteSource(sourceValue);
            }

            if (_localSource.HasValue())
            {
                foreach (var sourceValue in _localSource.Values)
                {
                    configuration.ForPackages.AddLocalSource(sourceValue);
                }
            }

            return configuration.CreateEngine(_productPath.Value(), _framework.Value());
        }
    }
}