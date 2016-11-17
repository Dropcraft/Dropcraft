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

        protected override void Define(CommandLineApplication app, Action<string> logErrorAction)
        {
            
        }

        protected override Task<int> Execute(CommandLineApplication app, Action<string> logErrorAction)
        {
            return Task.FromResult(0);
        }
    }
}