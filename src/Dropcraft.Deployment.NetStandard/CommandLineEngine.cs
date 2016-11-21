using System;
using System.Collections.Generic;
using Dropcraft.Common.Logging;
using Dropcraft.Deployment.Commands;
using Microsoft.Extensions.CommandLineUtils;

namespace Dropcraft.Deployment
{
    public class CommandLineEngine
    {
        private static readonly ILog Logger = LogProvider.For<CommandLineEngine>();

        protected CommandLineApplication App { get; }

        public string AppName { get; set; }

        public string AppFullName { get; set; }

        public string AppShortVersion { get; set; }
        public string AppLongVersion { get; set; }

        public CommandLineEngine(bool throwOnUnexpectedArg = true)
        {
            App = new CommandLineApplication(throwOnUnexpectedArg);
        }

        public int Run(params string[] args)
        {
            ConfigureApp();
            var commands = GetCommands();
            ConfigureCommands(commands);

            return Execute(args);
        }

        protected virtual void ConfigureApp()
        {
            App.Name = AppName;
            App.FullName = AppFullName;

            if (string.IsNullOrWhiteSpace(AppLongVersion))
                AppLongVersion = AppShortVersion;

            App.VersionOption("--version", AppShortVersion, AppLongVersion);
            App.HelpOption(CommandHelper.HelpOption);
        }

        protected virtual IEnumerable<DeploymentCommand> GetCommands()
        {
            return new DeploymentCommand[]
            {
                new InstallCommand(),
                new UninstallCommand(),
                new UpdateCommand(),
                new RepairCommand(),
                new ManifestCommand(),
                new InspectCommand(), 
            };
        }

        protected virtual void ConfigureCommands(IEnumerable<DeploymentCommand> commands)
        {
            foreach (var command in commands)
            {
                command.Register(App, x=>Logger.Error(x));
            }
        }

        protected virtual int Execute(params string[] args)
        {
            int exitCode;

            App.OnExecute(() =>
            {
                App.ShowHelp();
                return 0;
            });

            try
            {
                exitCode = App.Execute(args);
                if (exitCode < 0 || exitCode > 255)
                {
                    exitCode = 1;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e.Message);
                Logger.TraceException(e.Message, e);
                exitCode = 1;
            }

            return exitCode;
        }
    }
}