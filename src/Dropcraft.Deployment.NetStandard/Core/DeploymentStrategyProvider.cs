using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dropcraft.Common;
using Dropcraft.Common.Deployment;
using Dropcraft.Deployment.NuGet;

namespace Dropcraft.Deployment.Core
{
    /// <summary>
    /// Class DeploymentStrategyProvider.
    /// </summary>
    /// <seealso cref="Dropcraft.Common.Deployment.IDeploymentStrategyProvider" />
    public class DeploymentStrategyProvider : IDeploymentStrategyProvider
    {
        private readonly DeploymentContext _deploymentContext;

        /// <summary>
        /// Gets or sets the default conflict resolution.
        /// </summary>
        /// <value><see cref="FileConflictResolution"/></value>
        public FileConflictResolution DefaultConflictResolution { get; set; } = FileConflictResolution.Override;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeploymentStrategyProvider"/> class.
        /// </summary>
        /// <param name="deploymentContext">The deployment context.</param>
        public DeploymentStrategyProvider(DeploymentContext deploymentContext)
        {
            _deploymentContext = deploymentContext;
        }

        /// <summary>
        /// Returns files required for installation for the provided package
        /// </summary>
        /// <param name="packageId">Package been installed</param>
        /// <param name="packagePath">Path to the unpacked package</param>
        /// <returns>List of the files for deployment. <see cref="PackageFileDeploymentInfo"/></returns>
        public virtual IReadOnlyCollection<PackageFileDeploymentInfo> GetPackageFiles(PackageId packageId, string packagePath)
        {
            return OnGetPackageFiles(packageId, packagePath);
        }

        /// <summary>
        /// Called by <see cref="GetPackageFiles"/> method.
        /// </summary>
        /// <param name="packageId">The package identifier.</param>
        /// <param name="packagePath">The package path.</param>
        /// <returns>List of the files for deployment. <see cref="PackageFileDeploymentInfo"/></returns>
        protected IReadOnlyCollection<PackageFileDeploymentInfo> OnGetPackageFiles(PackageId packageId, string packagePath)
        {
            var files = new List<PackageFileDeploymentInfo>();
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
                        Directory.EnumerateDirectories(libFolder).Select(Path.GetFileName));
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

        /// <summary>
        /// Iterates through the package's files and adds them for deployment
        /// </summary>
        /// <param name="path">Current file search path</param>
        /// <param name="targetPath">Target installation path.</param>
        /// <param name="action">File action for the current folder</param>
        /// <param name="fileType">Type of the file</param>
        /// <param name="files">List of the files collected by the function</param>
        protected void AddFiles(string path, string targetPath, FileAction action, FileType fileType, List<PackageFileDeploymentInfo> files)
        {
            var fileEntries = Directory.EnumerateFiles(path);
            files.AddRange(fileEntries.Where(f=>Path.GetFileName(f) != "_._").Select(fileEntry => new PackageFileDeploymentInfo
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