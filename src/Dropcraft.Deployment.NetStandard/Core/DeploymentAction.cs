using System.Collections.Generic;
using System.Linq;
using Dropcraft.Common;
using Dropcraft.Common.Deployment;
using Dropcraft.Common.Logging;
using Dropcraft.Common.Package;
using Dropcraft.Deployment.NuGet;
using Dropcraft.Runtime.Core;

namespace Dropcraft.Deployment.Core
{
    /// <summary>
    /// A base class for all deployment actions
    /// </summary>
    public abstract class DeploymentAction
    {
        private static readonly ILog Logger = LogProvider.For<DeploymentAction>();

        /// <summary>
        /// Gets a value indicating whether this action is part of the package updating sequence
        /// </summary>
        /// <value><c>true</c> if this instance is update; otherwise, <c>false</c>.</value>
        public bool IsUpdate { get; }

        /// <summary>
        /// Gets the current deployment context.
        /// </summary>
        /// <value>The deployment context.</value>
        public DeploymentContext DeploymentContext { get; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="DeploymentAction"/> class.
        /// </summary>
        /// <param name="deploymentContext">The deployment context.</param>
        /// <param name="isUpdate">Is this action is part of update sequence</param>
        protected DeploymentAction(DeploymentContext deploymentContext, bool isUpdate)
        {
            IsUpdate = isUpdate;
            DeploymentContext = deploymentContext;
        }

        /// <summary>
        /// Raises the deployment event.
        /// </summary>
        /// <param name="e"><see cref="PackageDeploymentEvent"/></param>
        /// <param name="id">Package identifier.</param>
        public void RaiseDeploymentEvent(PackageDeploymentEvent e, PackageId id)
        {
            e.Id = id;
            e.IsUpdateInProgress = IsUpdate;
            DeploymentContext.RaiseDeploymentEvent(e);
        }

        /// <summary>
        /// Logs message using information logging level 
        /// </summary>
        /// <param name="message">The message.</param>
        protected void Info(string message)
        {
            Logger.Info(message);
        }

        /// <summary>
        /// Executes the action under the specified transaction.
        /// </summary>
        /// <param name="transaction">The transaction.</param>
        /// <seealso cref="IDeploymentTransaction"/>
        public abstract void Execute(IDeploymentTransaction transaction);
    }

    /// <summary>
    /// Defines an action for a single package downloading
    /// </summary>
    /// <seealso cref="Dropcraft.Deployment.Core.DeploymentAction" />
    public class DownloadPackageAction : DeploymentAction
    {
        private readonly INuGetEngine _nuGetEngine;
        private readonly string _path;
        private readonly DeploymentPackageInfo _deploymentPackage;

        /// <summary>
        /// Initializes a new instance of the <see cref="DownloadPackageAction"/> class.
        /// </summary>
        /// <param name="deploymentContext">The deployment context.</param>
        /// <param name="isUpdate"><see cref="DeploymentAction.IsUpdate"/></param>
        /// <param name="deploymentPackage">The associated package</param>
        /// <param name="nuGetEngine">NuGet engine to use</param>
        /// <param name="path">Download path</param>
        /// <seealso cref="DeploymentPackageInfo"/>
        /// <seealso cref="INuGetEngine"/>
        public DownloadPackageAction(DeploymentContext deploymentContext, bool isUpdate, DeploymentPackageInfo deploymentPackage,
            INuGetEngine nuGetEngine, string path) : base(deploymentContext, isUpdate)
        {
            _nuGetEngine = nuGetEngine;
            _path = path;
            _deploymentPackage = deploymentPackage;
        }

        /// <summary>
        /// Executes the action under the specified transaction.
        /// </summary>
        /// <param name="transaction">The transaction.</param>
        /// <seealso cref="IDeploymentTransaction" />
        public override void Execute(IDeploymentTransaction transaction)
        {
            _nuGetEngine.InstallPackage(_deploymentPackage.Match, _path).GetAwaiter().GetResult();
            _deploymentPackage.PackagePath = _nuGetEngine.GetPackageTargetPath(_deploymentPackage.Match.Library.Name,
                _deploymentPackage.Match.Library.Version, _path);

            RaiseDeploymentEvent(new AfterPackageDownloadedEvent { PackagePath = _deploymentPackage.PackagePath }, _deploymentPackage.Id);
            Info($"{_deploymentPackage.Id} downloaded");
        }
    }

    /// <summary>
    /// Defines an action for a single package deletion
    /// </summary>
    /// <seealso cref="Dropcraft.Deployment.Core.DeploymentAction" />
    public class DeletePackageAction : DeploymentAction
    {
        private readonly PackageId _packageId;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeletePackageAction"/> class.
        /// </summary>
        /// <param name="deploymentContext">The deployment context.</param>
        /// <param name="isUpdate"><see cref="DeploymentAction.IsUpdate"/></param>
        /// <param name="packageId">The package identifier.</param>
        public DeletePackageAction(DeploymentContext deploymentContext, bool isUpdate, PackageId packageId)
            : base(deploymentContext, isUpdate)
        {
            _packageId = packageId;
        }

        /// <summary>
        /// Executes the action under the specified transaction.
        /// </summary>
        /// <param name="transaction">The transaction.</param>
        /// <seealso cref="IDeploymentTransaction" />
        public override void Execute(IDeploymentTransaction transaction)
        {
            var files = DeploymentContext.ProductConfigurationProvider.GetInstalledFiles(_packageId, true);

            var deleteEvent = new BeforePackageUninstalledEvent();
            deleteEvent.FilesToDelete.AddRange(files);
            RaiseDeploymentEvent(deleteEvent, _packageId);

            foreach (var file in deleteEvent.FilesToDelete)
            {
                transaction.DeleteFile(file);
            }

            DeploymentContext.ProductConfigurationProvider.RemovePackage(_packageId);
            transaction.TrackDeletedPackage(_packageId);

            RaiseDeploymentEvent(new AfterPackageUninstalledEvent(), _packageId);
            Info($"{_packageId} uninstalled");
        }
    }

    /// <summary>
    /// Defines an action for a single package installation
    /// </summary>
    /// <seealso cref="Dropcraft.Deployment.Core.DeploymentAction" />
    public class InstallPackageAction : DeploymentAction
    {
        private readonly DeploymentPackageInfo _deploymentPackage;
        private readonly IPackageConfigurationProvider _packageConfigProvider;
        private readonly IDeploymentStrategyProvider _deploymentStrategy;

        /// <summary>
        /// Initializes a new instance of the <see cref="InstallPackageAction"/> class.
        /// </summary>
        /// <param name="deploymentContext">The deployment context.</param>
        /// <param name="isUpdate"><see cref="DeploymentAction.IsUpdate"/></param>
        /// <param name="deploymentPackage">The deployment package.</param>
        /// <param name="deploymentStrategy">The deployment strategy.</param>
        /// <seealso cref="DeploymentPackageInfo"/>
        public InstallPackageAction(DeploymentContext deploymentContext, bool isUpdate, DeploymentPackageInfo deploymentPackage,
            IDeploymentStrategyProvider deploymentStrategy) : base(deploymentContext, isUpdate)
        {
            _deploymentPackage = deploymentPackage;
            _packageConfigProvider = DeploymentContext.PackageConfigurationProvider;
            _deploymentStrategy = deploymentStrategy;
        }

        /// <summary>
        /// Executes the action under the specified transaction.
        /// </summary>
        /// <param name="transaction">The transaction.</param>
        /// <seealso cref="IDeploymentTransaction" />
        public override void Execute(IDeploymentTransaction transaction)
        {
            var files = _deploymentStrategy.GetPackageFiles(_deploymentPackage.Id, _deploymentPackage.PackagePath).ToList();
            var installedFiles = new List<PackageFileInfo>();

            var e = new BeforePackageInstalledEvent();
            e.FilesToInstall.AddRange(files);
            RaiseDeploymentEvent(e, _deploymentPackage.Id);

            var productPathLength = DeploymentContext.ProductPath.Length;

            foreach (var file in e.FilesToInstall)
            {
                if (file.Action == FileAction.Copy)
                {
                    transaction.InstallFile(file);

                    var filePath = file.TargetFileName;
                    if (filePath.StartsWith(DeploymentContext.ProductPath))
                    {
                        filePath = filePath.Remove(0, productPathLength).TrimStart('/', '\\');
                    }

                    installedFiles.Add(new PackageFileInfo(filePath));
                }
                else if (file.Action == FileAction.Delete)
                {
                    transaction.DeleteFile(new PackageFileInfo(file.TargetFileName));
                }
            }

            var cfg = _packageConfigProvider.GetPackageConfiguration(_deploymentPackage.Id, _deploymentPackage.PackagePath);
            transaction.TrackInstalledPackage(cfg, installedFiles);

            RaiseDeploymentEvent(new AfterPackageInstalledEvent(), _deploymentPackage.Id);
            Info($"{_deploymentPackage.Id} installed");
        }
    }
}