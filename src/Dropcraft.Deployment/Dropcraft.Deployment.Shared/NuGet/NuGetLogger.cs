
using System.Diagnostics;
using Dropcraft.Common.Logging;
using NuGet.Common;

namespace Dropcraft.Deployment.NuGet
{
    internal class NuGetLogger : ILogger
    {
        private static readonly ILog Logger = LogProvider.For<NuGetLogger>();

        public void LogDebug(string data) => Logger.Debug(data);

        public void LogVerbose(string data) => Logger.Trace(data);

        public void LogInformation(string data) => Logger.Info(data);

        public void LogMinimal(string data) => Logger.Trace(data);

        public void LogWarning(string data) => Logger.Warn(data);

        public void LogError(string data) => Logger.Error(data);

        public void LogInformationSummary(string data) => Logger.Info(data);

        public void LogErrorSummary(string data) => Logger.Error(data);
    }
}
