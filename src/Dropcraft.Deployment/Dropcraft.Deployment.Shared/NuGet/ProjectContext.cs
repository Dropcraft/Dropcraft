using System.Xml.Linq;
using Dropcraft.Common.Diagnostics;
using NuGet.Packaging;
using NuGet.ProjectManagement;

namespace Dropcraft.Deployment.NuGet
{
    internal class ProjectContext : INuGetProjectContext
    {
        public void Log(MessageLevel level, string message, params object[] args)
        {
            switch (level)
            {
                case MessageLevel.Warning:
                    Trace.Current.Warning(message, args);
                    break;
                case MessageLevel.Error:
                    Trace.Current.Error(message, args);
                    break;
                default:
                    Trace.Current.Verbose(message, args);
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
            Trace.Current.Verbose(message);
        }

        public NuGetActionType ActionType { get; set; }
    }
}
