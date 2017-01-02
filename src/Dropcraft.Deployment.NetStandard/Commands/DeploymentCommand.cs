using System;
using System.Threading.Tasks;
using Microsoft.Extensions.CommandLineUtils;

namespace Dropcraft.Deployment.Commands
{
    /// <summary>
    /// Base class for all deployment commands
    /// </summary>
    public abstract class DeploymentCommand
    {
        /// <summary>
        /// Gets or sets the command's name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Registers the command with the specified application.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="logErrorAction">The log error action.</param>
        public void Register(CommandLineApplication app, Action<string> logErrorAction)
        {
            app.Command(Name, cmdApp =>
            {
                Define(cmdApp, logErrorAction);
                cmdApp.OnExecute(async () => await Execute(cmdApp, logErrorAction));

            });
        }

        /// <summary>
        /// Allows to define the command
        /// </summary>
        /// <param name="app">The command line application.</param>
        /// <param name="logErrorAction">The log error action.</param>
        protected abstract void Define(CommandLineApplication app, Action<string> logErrorAction);

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="logErrorAction">The log error action.</param>
        /// <returns>Error code</returns>
        protected abstract Task<int> Execute(CommandLineApplication app, Action<string> logErrorAction);

        /// <summary>
        /// Informs user about a missed value for the option.
        /// </summary>
        /// <param name="option">The affected option.</param>
        /// <param name="logErrorAction">The log error action.</param>
        /// <returns><c>true</c> if value for the option is missed, <c>false</c> otherwise.</returns>
        protected bool MissedOption(CommandOption option, Action<string> logErrorAction)
        {
            if (option.HasValue())
                return false;

            logErrorAction($"Missed option: {option.ValueName}");
            return true;
        }

    }
}
