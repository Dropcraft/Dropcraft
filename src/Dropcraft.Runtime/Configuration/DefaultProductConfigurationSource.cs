namespace Dropcraft.Runtime.Configuration
{
    public class DefaultProductConfigurationSource
    {
        /// <summary>
        /// Defines application configuration file name to store information about installed packages, dependencies, etc.
        /// </summary>
        public string ApplicationConfigurationFileName { get; set; } = "dropcraft.json";

    }
}