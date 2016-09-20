using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dropcraft.Common;
using Dropcraft.Common.Logging;
using Dropcraft.Common.Package;
using Dropcraft.Deployment.Exceptions;
using Dropcraft.Deployment.NuGet;

namespace Dropcraft.Deployment
{
    public class DeploymentEngine : IDeploymentEngine
    {
        public DeploymentContext DeploymentContext { get; }
        private readonly NuGetEngine _nuGetEngine;
        private readonly List<IDeploymentFilter> _deploymentFilters;

        private static readonly ILog Logger = LogProvider.For<DeploymentEngine>();

        public DeploymentEngine(DeploymentConfiguration configuration)
        {
            DeploymentContext = configuration.DeploymentContext;
            _deploymentFilters = new List<IDeploymentFilter>(configuration.DeploymentFilters);
            _nuGetEngine = new NuGetEngine(configuration);
        }

        public async Task InstallPackages(IEnumerable<VersionedPackageInfo> packages)
        {
            var installedPackages = new List<InstallablePackage>();
            var installablePackages = await Task.WhenAll(packages.Select(_nuGetEngine.ResolveInstallablePackage));

            foreach (var package in installablePackages)
            {
                if (!package.IsResolved) continue;

                var processedPackages = await _nuGetEngine.InstallPackage(package);
                if (processedPackages.Count != 0)
                    installedPackages.AddRange(processedPackages);
            }

            if (!Directory.Exists(DeploymentContext.InstallPath))
                Directory.CreateDirectory(DeploymentContext.InstallPath);

            installedPackages.ForEach(ProcessInstallablePackage);
        }

        protected virtual void ProcessInstallablePackage(InstallablePackageInfo package)
        {
            Logger.Trace($"Processing package {package.Id} {package.ResolvedVersion}");
            foreach (var file in package.InstallableFiles)
            {
                var fileName = Path.GetFileName(file.FilePath);
                if (string.IsNullOrWhiteSpace(fileName))
                    continue;

                file.TargetFilePath = Path.Combine(DeploymentContext.InstallPath, fileName);
                file.Conflict = File.Exists(file.TargetFilePath);
            }

            foreach (var deploymentFilter in _deploymentFilters)
            {
                Logger.Trace($"Run filter {deploymentFilter} for {package.Id}");
                deploymentFilter.Filter(package);
            }

            var fileWithConflict = package.InstallableFiles.FirstOrDefault(
                x => x.Conflict && x.ConflictResolution == FileConflictResolution.Fail);

            if (fileWithConflict != null)
            {
                var msg =
                    $"Deployment stopped because of file conflict between {fileWithConflict.FilePath} and {fileWithConflict.TargetFilePath}";
                Logger.Error(msg);
                throw new FileConflictException(msg);
            }

            //TODO: read list of configuration files and mark file as config if needed

            CopyFiles(package);

            //TODO: call package deployed
            //TODO: reconfigure config files

        }

        protected virtual void CopyFiles(InstallablePackageInfo package)
        {
            foreach (var file in package.InstallableFiles)
            {
                Logger.Trace($"Copy file {file.FilePath} to {file.TargetFilePath}");
                if (file.Conflict)
                {
                    Logger.Trace($"Conflict between {file.FilePath} and {file.TargetFilePath}");
                    if (file.ConflictResolution == FileConflictResolution.KeepExisting)
                    {
                        Logger.Trace("Conflict resolved: keep existing file");
                        return;
                    }

                    Logger.Trace("Conflict resolved: override file");
                }

                try
                {
                    File.Copy(file.FilePath, file.TargetFilePath, true);
                }
                catch (Exception exception)
                {
                    Logger.Trace($"Copy failed for {file.FilePath}, {exception.Message}, {exception.StackTrace}");
                    throw;
                }
            }
        }

        public async Task UpdatePackages(IEnumerable<VersionedPackageInfo> packages)
        {
        }

        public async Task UninstallPackages(IEnumerable<VersionedPackageInfo> packages)
        {
        }


    }
}
