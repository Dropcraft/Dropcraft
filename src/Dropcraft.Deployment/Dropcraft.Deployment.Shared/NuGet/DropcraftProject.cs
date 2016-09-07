using System.Threading;
using System.Threading.Tasks;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.ProjectManagement;
using NuGet.Protocol.Core.Types;

namespace Dropcraft.Deployment.NuGet
{
    internal class DropcraftProject : FolderNuGetProject
    {
        public DropcraftProject(string root) : base(root)
        {
        }

        public DropcraftProject(string root, bool excludeVersion) : base(root, excludeVersion)
        {
        }

        public DropcraftProject(string root, PackagePathResolver packagePathResolver) : base(root, packagePathResolver)
        {
        }

        public override Task<bool> InstallPackageAsync(PackageIdentity packageIdentity, DownloadResourceResult downloadResourceResult,
            INuGetProjectContext nuGetProjectContext, CancellationToken token)
        {
            // TODO: register package
            return base.InstallPackageAsync(packageIdentity, downloadResourceResult, nuGetProjectContext, token);
        }
    }
}
