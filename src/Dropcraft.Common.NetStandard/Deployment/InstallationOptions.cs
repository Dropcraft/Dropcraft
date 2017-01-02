namespace Dropcraft.Common.Deployment
{
    /// <summary>
    /// Options for <see cref="IDeploymentEngine"/> InstallPackages() method
    /// </summary>
    public class InstallationOptions
    {
        /// <summary>
        /// Instructs to allow packages downgrades. Default value is false.
        /// </summary>
        public bool AllowDowngrades { get; set; } = false;

        /// <summary>
        /// When true, packages will be always updated from the remote source, even if they can be resolved from the installed path. Default value is true.
        /// </summary>
        public bool UpdatePackages { get; set; } = true;

        /// <summary>
        /// When true, only the listed packages will be installed, without their dependencies. Default value is false.
        /// </summary>
        public bool SkipDependencies { get; set; } = false;
    }
}