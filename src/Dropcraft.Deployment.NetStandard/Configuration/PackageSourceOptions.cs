namespace Dropcraft.Deployment.Configuration
{
    public class PackageSourceOptions
    {
        readonly DeploymentConfiguration _configuration;

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
        /// Configure engine to cache downloaded in the provided folder
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