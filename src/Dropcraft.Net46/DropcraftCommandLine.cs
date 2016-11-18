using System.Reflection;
using Dropcraft.Deployment;
using Microsoft.Extensions.CommandLineUtils;

namespace Dropcraft
{
    public class DropcraftCommandLine : CommandLineEngine
    {
        public DropcraftCommandLine()
        {
            AppFullName = "Dropcraft";
            AppName = "dropcraft";
            AppShortVersion = Assembly.GetCallingAssembly().GetName().Version.ToString();
        }

        protected override void ConfigureApp()
        {
            base.ConfigureApp();

            App.Option("--verbose", "Increase log verbosity", CommandOptionType.NoValue);
            App.Option("--debug", "Pause the application on startup and allow to attach a debuger", CommandOptionType.NoValue);
        }
    }
}