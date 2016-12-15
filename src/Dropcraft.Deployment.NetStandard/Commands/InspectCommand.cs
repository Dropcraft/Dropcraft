using System;
using System.Threading.Tasks;
using Dropcraft.Common;
using Microsoft.Extensions.CommandLineUtils;

namespace Dropcraft.Deployment.Commands
{
    public class InspectCommand : DeploymentCommand
    {
        private CommandOption _productPath;

        public InspectCommand()
        {
            Name = "inspect";
        }

        protected override void Define(CommandLineApplication cmdApp, Action<string> logErrorAction)
        {
            cmdApp.Description = "Provides information about the product";
            cmdApp.HelpOption(CommandHelper.HelpOption);

            _productPath = cmdApp.Option("--path <installationPath>", "Product installation path", CommandOptionType.SingleValue);
        }

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
                Console.WriteLine(packageConfiguration.Id.ToString());
            }

            return await Task.FromResult(0);
        }

        protected virtual IDeploymentEngine GetDeploymentEngine(CommandLineApplication app)
        {
            return CommandHelper.GetConfiguration().CreatEngine(_productPath.Value(), string.Empty);
        }

    }
}