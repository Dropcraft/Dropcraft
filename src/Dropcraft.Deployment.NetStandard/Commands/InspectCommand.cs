using System;
using System.Threading.Tasks;
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

            return await Task.FromResult(0);
        }
    }
}