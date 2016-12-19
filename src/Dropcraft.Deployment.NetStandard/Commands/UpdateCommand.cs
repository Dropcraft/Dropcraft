using System;
using System.Linq;
using System.Threading.Tasks;
using Dropcraft.Common;
using Dropcraft.Common.Deployment;
using Dropcraft.Common.Package;
using Microsoft.Extensions.CommandLineUtils;

namespace Dropcraft.Deployment.Commands
{
    public class UpdateCommand : DeploymentCommand
    {
        private CommandOption _productPath;
        private CommandOption _packagePath;
        private CommandOption _framework;
        private CommandOption _source;
        private CommandOption _allowPrerelease;

        public UpdateCommand()
        {
            Name = "update";
        }

        protected override void Define(CommandLineApplication cmdApp, Action<string> logErrorAction)
        {
            cmdApp.Description = "Updates all the product packages";
            cmdApp.HelpOption(CommandHelper.HelpOption);

            _productPath = cmdApp.Option("--path <installationPath>", "Product installation path", CommandOptionType.SingleValue);
            _packagePath = cmdApp.Option("--packages <packagesDirectory>", "Packages installation path, used as a local packages cache", CommandOptionType.SingleValue);
            _framework = cmdApp.Option("--framework <frameworkId>", "Target framework ID", CommandOptionType.SingleValue);
            _source = cmdApp.Option("-s|--source <packageSource>", "Remote package source, URL or path", CommandOptionType.MultipleValue);
            _allowPrerelease = cmdApp.Option("-p|--allow-prerelease", "Allow to update to prerelease version", CommandOptionType.NoValue);

        }

        protected override async Task<int> Execute(CommandLineApplication cmdApp, Action<string> logErrorAction)
        {
            if (MissedOption(_productPath, logErrorAction))
                return 1;

            if (MissedOption(_framework, logErrorAction))
                return 1;

            if (MissedOption(_source, logErrorAction))
                return 1;

            var engine = GetDeploymentEngine(cmdApp);
            if (!engine.DeploymentContext.ProductConfigurationProvider.IsProductConfigured)
            {
                logErrorAction($"Configuration not found at {_productPath}");
                return 1;
            }

            var packages =
                engine.DeploymentContext.ProductConfigurationProvider.GetPackages()
                    .Packages.Select(x => new PackageId(x.Package.Id, string.Empty, _allowPrerelease.HasValue()))
                    .ToArray();

            await engine.InstallPackages(packages, false, true);

            return 0;
        }

        protected virtual IDeploymentEngine GetDeploymentEngine(CommandLineApplication app)
        {
            var configuration = CommandHelper.GetConfiguration()
                .ForPackages.Cache(_packagePath.HasValue() ? _packagePath.Value() : string.Empty);

            foreach (var sourceValue in _source.Values)
            {
                configuration.ForPackages.AddRemoteSource(sourceValue);
            }

            return configuration.CreatEngine(_productPath.Value(), _framework.Value());
        }
    }
}