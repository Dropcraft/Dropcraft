
using Dropcraft.Common.Diagnostics;
using NuGet.Common;

namespace Dropcraft.Deployment.NuGet
{
    internal class NuGetLogger : ILogger
    {
        public void LogDebug(string data) => Trace.Current.Verbose(data);

        public void LogVerbose(string data) => Trace.Current.Verbose(data);

        public void LogInformation(string data) => Trace.Current.Verbose(data);

        public void LogMinimal(string data) => Trace.Current.Verbose(data);

        public void LogWarning(string data) => Trace.Current.Warning(data);

        public void LogError(string data) => Trace.Current.Error(data);

        public void LogInformationSummary(string data) => Trace.Current.Verbose(data);

        public void LogErrorSummary(string data) => Trace.Current.Verbose(data);
    }
}
