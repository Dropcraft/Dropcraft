using Microsoft.Extensions.CommandLineUtils;

namespace Dropcraft.Deployment.Commands
{
    public abstract class DeploymentCommand
    {
        public abstract void Register(CommandLineApplication app);
    }
}
