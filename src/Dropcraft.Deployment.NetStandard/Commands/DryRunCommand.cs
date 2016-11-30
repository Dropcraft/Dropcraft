using System;
using System.Threading.Tasks;
using Microsoft.Extensions.CommandLineUtils;

namespace Dropcraft.Deployment.Commands
{
    public class DryRunCommand : DeploymentCommand
    {
        public DryRunCommand()
        {
            Name = "dryrun";
        }

        protected override void Define(CommandLineApplication cmdApp, Action<string> logErrorAction)
        {
            cmdApp.Description = "Creates a product configuration file without installing the packages";
            cmdApp.HelpOption(CommandHelper.HelpOption);
        }

        protected override async Task<int> Execute(CommandLineApplication app, Action<string> logErrorAction)
        {
            return await Task.FromResult(0);
        }
    }
}