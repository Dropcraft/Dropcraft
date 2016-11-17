using System.Reflection;
using Dropcraft.Deployment;
using Serilog;

namespace Dropcraft
{
    class Program
    {
        static int Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.ColoredConsole(outputTemplate:"{Message}{NewLine}{Exception}")
                .CreateLogger();

            var engine = new CommandLineEngine
            {
                AppFullName = "Dropcraft",
                AppName = "dropcraft",
                AppShortVersion = Assembly.GetCallingAssembly().GetName().Version.ToString(),
            };

            return engine.Run(args);
        }
    }
}
