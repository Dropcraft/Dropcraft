using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.CommandLineUtils;

namespace Dropcraft.Deployment.Commands
{
    public class ManifestCommand : DeploymentCommand
    {
        private CommandArgument _path;

        public ManifestCommand()
        {
            Name = "manifest";
        }

        protected override void Define(CommandLineApplication cmdApp, Action<string> logErrorAction)
        {
            cmdApp.Description = "Creates an empty package manifest";
            cmdApp.HelpOption(CommandHelper.HelpOption);

            _path = cmdApp.Argument("[path]", "Folder where the newly created manifest will be stored. E.g. C:\\Projects\\MyApp");
        }

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