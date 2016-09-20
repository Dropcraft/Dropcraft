using System.Diagnostics;
using System.Xml.Linq;
using Dropcraft.Common.Logging;
using NuGet.Packaging;
using NuGet.ProjectManagement;

namespace Dropcraft.Deployment.NuGet
{
    internal class ProjectContext : INuGetProjectContext
    {
        private static readonly ILog Logger = LogProvider.For<NuGetLogger>();

        public void Log(MessageLevel level, string message, params object[] args)
        {
            switch (level)
            {
                case MessageLevel.Warning:
                    Logger.WarnFormat(message, args);
                    break;
                case MessageLevel.Error:
                    Logger.ErrorFormat(message, args);
                    break;
                default:
                    Logger.TraceFormat(message, args);
                    break;
            }
        }

        public FileConflictAction ResolveFileConflict(string message) => FileConflictAction.Ignore;

        public PackageExtractionContext PackageExtractionContext { get; set; }

        public XDocument OriginalPackagesConfig { get; set; }

        public ISourceControlManagerProvider SourceControlManagerProvider => null;

        public ExecutionContext ExecutionContext => null;

        public void ReportError(string message)
        {
            Logger.Error(message);
        }

        public NuGetActionType ActionType { get; set; }
    }
}
