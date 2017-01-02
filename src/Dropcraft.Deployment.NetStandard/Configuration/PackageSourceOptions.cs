namespace Dropcraft.Deployment.Configuration
{
    /// <summary>
    /// Class PackageSourceOptions.
    /// </summary>
    public class PackageSourceOptions
    {
        readonly DeploymentConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="PackageSourceOptions"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public PackageSourceOptions(DeploymentConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Configures engine to use additional remote packages source
        /// </summary>
        /// <param name="source">Packages source</param>
        /// <returns>Configuration object</returns>
        public DeploymentConfiguration AddRemoteSource(string source)
        {
            _configuration.RemotePackagesSources.Add(source);
            return _configuration;
        }

        /// <summary>
        /// Configures engine to use additional local packages source
        /// </summary>
        /// <param name="source">Packages source</param>
        /// <returns>Configuration object</returns>
        public DeploymentConfiguration AddLocalSource(string source)
        {
            _configuration.LocalPackagesSources.Add(source);
            return _configuration;
        }

        /// <summary>
        /// Configure engine to cache downloaded packages in the provided folder
        /// </summary>
        /// <param name="folderPath">Target folder</param>
        /// <returns>Configuration object</returns>
        public DeploymentConfiguration Cache(string folderPath)
        {
            _configuration.PackagesFolderPath = folderPath;
            return _configuration;
        }

    }
}