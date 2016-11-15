using Microsoft.Extensions.CommandLineUtils;

namespace Dropcraft.Deployment.Commands
{
    public class InstallCommand : DeploymentCommand
    {
        public override void Register(CommandLineApplication app)
        {
            app.Command("install", cmdApp =>
            {
                //cmdApp.Description
                cmdApp.HelpOption(CommandHelper.HelpOption);

                var productPath = cmdApp.Option("--path <targetPath>", "description", CommandOptionType.SingleValue);
            });
        }
    }
}