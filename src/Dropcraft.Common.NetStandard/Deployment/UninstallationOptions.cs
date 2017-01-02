namespace Dropcraft.Common.Deployment
{
    /// <summary>
    /// Options for <see cref="IDeploymentEngine"/> UninstallPackages() method
    /// </summary>
    public class UninstallationOptions
    {
        /// <summary>
        /// When true, instructs to remove dependent packages if they are not referenced elsewhere. Default value is true.
        /// </summary>
        public bool RemoveDependencies { get; set; } = true;
    }
}