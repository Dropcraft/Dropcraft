using System;
using System.Diagnostics;
using System.Linq;
using Serilog;
using Serilog.Events;

namespace Dropcraft
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Contains("--debug"))
            {
                Console.WriteLine("Waiting for debugger to attach.");
                Console.WriteLine($"Process ID: {Process.GetCurrentProcess().Id}");

                while (!Debugger.IsAttached)
                {
                    System.Threading.Thread.Sleep(100);
                }

                Debugger.Break();
            }

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Is(args.Contains("--verbose") ? LogEventLevel.Verbose : LogEventLevel.Information)
                .WriteTo.ColoredConsole(outputTemplate:"{Message}{NewLine}{Exception}")
                .CreateLogger();

            var engine = new DropcraftCommandLine();
            return engine.Run(args);
        }
    }
}
