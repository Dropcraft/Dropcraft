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

        protected override void Define(CommandLineApplication app, Action<string> logErrorAction)
        {
            
        }

        protected override Task<int> Execute(CommandLineApplication app, Action<string> logErrorAction)
        {
            return Task.FromResult(0);
        }
    }
}