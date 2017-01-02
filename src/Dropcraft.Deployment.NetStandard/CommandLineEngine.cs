using System;
using System.Collections.Generic;
using Dropcraft.Common.Logging;
using Dropcraft.Deployment.Commands;
using Microsoft.Extensions.CommandLineUtils;

namespace Dropcraft.Deployment
{
    /// <summary>
    /// Class CommandLineEngine.
    /// </summary>
    public class CommandLineEngine
    {
        private static readonly ILog Logger = LogProvider.For<CommandLineEngine>();

        /// <summary>
        /// Current command line application
        /// </summary>
        protected CommandLineApplication App { get; }

        /// <summary>
        /// Gets or sets the short name of the application.
        /// </summary>
        /// <value>The short name of the application.</value>
        public string AppName { get; set; }
        
        /// <summary>
        /// Gets or sets the full name of the application.
        /// </summary>
        /// <value>The full name of the application.</value>
        public string AppFullName { get; set; }

        /// <summary>
        /// Gets or sets the application short version.
        /// </summary>
        /// <value>The application short version.</value>
        public string AppShortVersion { get; set; }

        /// <summary>
        /// Gets or sets the application long version.
        /// </summary>
        /// <value>The application long version.</value>
        public string AppLongVersion { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLineEngine"/> class.
        /// </summary>
        /// <param name="throwOnUnexpectedArg">if set to <c>true</c> an exception will be thrown for any unexpected arguments</param>
        public CommandLineEngine(bool throwOnUnexpectedArg = true)
        {
            App = new CommandLineApplication(throwOnUnexpectedArg);
        }

        /// <summary>
        /// Runs the application
        /// </summary>
        /// <param name="args">Application arguments</param>
        /// <returns>Error code</returns>
        public int Run(params string[] args)
        {
            ConfigureApp();
            var commands = GetCommands();
            ConfigureCommands(commands);

            return OnRun(args);
        }

        /// <summary>
        /// Allows to pre-configure the application in the derived classes
        /// </summary>
        protected virtual void ConfigureApp()
        {
            App.Name = AppName;
            App.FullName = AppFullName;

            if (string.IsNullOrWhiteSpace(AppLongVersion))
                AppLongVersion = AppShortVersion;

            App.VersionOption("--version", AppShortVersion, AppLongVersion);
            App.HelpOption(CommandHelper.HelpOption);
        }

        /// <summary>
        /// Returns the list of the supported commands
        /// </summary>
        /// <returns>List of <see cref="DeploymentCommand"/></returns>
        protected virtual IEnumerable<DeploymentCommand> GetCommands()
        {
            return new DeploymentCommand[]
            {
                new InstallCommand(),
                new UninstallCommand(),
                new UpdateCommand(),
                new RepairCommand(),
                new ManifestCommand(),
                new InspectCommand()
            };
        }

        /// <summary>
        /// Registers the commands.
        /// </summary>
        /// <param name="commands">Commands</param>
        protected virtual void ConfigureCommands(IEnumerable<DeploymentCommand> commands)
        {
            foreach (var command in commands)
            {
                command.Register(App, x=>Logger.Error(x));
            }
        }

        /// <summary>
        /// Runs the application
        /// </summary>
        /// <param name="args">The application arguments.</param>
        /// <returns>Error code</returns>
        /// <seealso cref="Run"/>
        protected virtual int OnRun(params string[] args)
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