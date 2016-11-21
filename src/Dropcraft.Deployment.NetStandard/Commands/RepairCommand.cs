using System;
using System.Threading.Tasks;
using Microsoft.Extensions.CommandLineUtils;

namespace Dropcraft.Deployment.Commands
{
    public class RepairCommand : DeploymentCommand
    {
        public RepairCommand()
        {
            Name = "repair";
        }

        protected override void Define(CommandLineApplication cmdApp, Action<string> logErrorAction)
        {
            cmdApp.Description = "Repairs the product by reinstalling all the packages";
            cmdApp.HelpOption(CommandHelper.HelpOption);
        }

        protected override Task<int> Execute(CommandLineApplication app, Action<string> logErrorAction)
        {
            return Task.FromResult(0);
        }
    }
}