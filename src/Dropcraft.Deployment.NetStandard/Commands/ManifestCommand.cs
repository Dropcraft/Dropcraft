using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.CommandLineUtils;

namespace Dropcraft.Deployment.Commands
{
    /// <summary>
    /// Class ManifestCommand.
    /// </summary>
    /// <seealso cref="Dropcraft.Deployment.Commands.DeploymentCommand" />
    public class ManifestCommand : DeploymentCommand
    {
        private CommandArgument _path;

        /// <summary>
        /// Initializes a new instance of the <see cref="ManifestCommand"/> class.
        /// </summary>
        public ManifestCommand()
        {
            Name = "manifest";
        }

        /// <summary>
        /// Defines the specified command application.
        /// </summary>
        /// <param name="cmdApp">The command application.</param>
        /// <param name="logErrorAction">The log error action.</param>
        protected override void Define(CommandLineApplication cmdApp, Action<string> logErrorAction)
        {
            cmdApp.Description = "Creates an empty package manifest";
            cmdApp.HelpOption(CommandHelper.HelpOption);

            _path = cmdApp.Argument("[path]", "Folder where the newly created manifest will be stored. E.g. C:\\Projects\\MyApp");
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="logErrorAction">The log error action.</param>
        /// <returns>Error code</returns>
        protected override async Task<int> Execute(CommandLineApplication app, Action<string> logErrorAction)
        {
            var assembly = GetType().GetTypeInfo().Assembly;
            using (var stream = assembly.GetManifestResourceStream("Dropcraft.Deployment.Commands.manifest.json"))
            {
                using (var sr = new StreamReader(stream))
                {
                    var filePath = string.IsNullOrWhiteSpace(_path.Value)
                        ? "manifest.json"
                        : Path.Combine(_path.Value, "manifest.json");

                    using (var sw = File.CreateText(filePath))
                    {
                        sw.Write(sr.ReadToEnd());
                        sw.Flush();
                    }
                }
            }

            return await Task.FromResult(0);
        }
    }
}