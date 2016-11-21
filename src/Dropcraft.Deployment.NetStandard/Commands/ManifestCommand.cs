using System;
using System.Threading.Tasks;
using Microsoft.Extensions.CommandLineUtils;

namespace Dropcraft.Deployment.Commands
{
    public class ManifestCommand : DeploymentCommand
    {
        public ManifestCommand()
        {
            Name = "manifest";
        }

        protected override void Define(CommandLineApplication cmdApp, Action<string> logErrorAction)
        {
            cmdApp.Description = "Creates an empty package manifest";
            cmdApp.HelpOption(CommandHelper.HelpOption);
        }

        protected override Task<int> Execute(CommandLineApplication app, Action<string> logErrorAction)
        {
            return Task.FromResult(0);
        }
    }
}