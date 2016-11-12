using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dropcraft.Common;
using Dropcraft.Common.Configuration;
using Dropcraft.Deployment.NuGet;

namespace Dropcraft.Deployment.Configuration
{
    public class DeploymentStarategyProvider : IDeploymentStartegyProvider
    {
        private readonly DeploymentContext _deploymentContext;

        public FileConflictResolution DefaultConflictResolution { get; set; } = FileConflictResolution.Override;

        public DeploymentStarategyProvider(DeploymentContext deploymentContext)
        {
            _deploymentContext = deploymentContext;
        }

        public virtual IEnumerable<PackageFileInfo> GetPackageFiles(PackageId packageId, string packagePath)
        {
            return OnGetPackageFiles(packageId, packagePath);
        }

        protected IEnumerable<PackageFileInfo> OnGetPackageFiles(PackageId packageId, string packagePath)
        {
            var files = new List<PackageFileInfo>();
            var toolsFolder = Path.Combine(packagePath, "tools");
            var contentFolder = Path.Combine(packagePath, "content");
            var libFolder = Path.Combine(packagePath, "lib");

            if (Directory.Exists(toolsFolder))
            {
                AddFiles(toolsFolder, _deploymentContext.ProductPath, FileAction.None, FileType.Tool, files);
            }

            if (Directory.Exists(contentFolder))
            {
                AddFiles(contentFolder, _deploymentContext.ProductPath, FileAction.Copy, FileType.Content, files);
            }

            if (Directory.Exists(libFolder))
            {
                var libFxFolder = Path.Combine(libFolder, _deploymentContext.TargetFramework);
                if (!Directory.Exists(libFxFolder))
                {
                    var fxName = NuGetEngine.GetMostCompatibleFramework(_deploymentContext.TargetFramework,
                        Directory.EnumerateDirectories(libFolder));
                    libFxFolder = Path.Combine(libFolder, fxName);
                    if (!Directory.Exists(libFxFolder))
                    {
                        return files;
                    }
                }

                AddFiles(libFxFolder, _deploymentContext.ProductPath, FileAction.Copy, FileType.LibFile, files);
            }

            return files;
        }

        protected void AddFiles(string path, string targetPath, FileAction action, FileType fileType, List<PackageFileInfo> files)
        {
            var fileEntries = Directory.EnumerateFiles(path);
            files.AddRange(fileEntries.Select(fileEntry => new PackageFileInfo
            {
                Action = action,
                ConflictResolution = DefaultConflictResolution,
                FileName = fileEntry,
                TargetFileName = Path.Combine(targetPath, Path.GetFileName(fileEntry)),
                FileType = fileType
            }));

            var directories = Directory.EnumerateDirectories(path);
            foreach (var dir in directories)
            {
                var newTargetPath = Path.Combine(targetPath, Path.GetFileName(dir));
                AddFiles(dir, newTargetPath, action, fileType, files);
            }

        }
    }
}