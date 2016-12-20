using Dropcraft.Common.Package;

namespace Dropcraft.Runtime.Configuration
{
    public class ExtensibilityOptions
    {
        private readonly RuntimeConfiguration _runtimeConfiguration;

        internal ExtensibilityOptions(RuntimeConfiguration runtimeConfiguration)
        {
            _runtimeConfiguration = runtimeConfiguration;
        }

        /// <summary>
        /// Allows to setup a custom activator for the package entities
        /// </summary>
        /// <param name="entityActivator">Custom activator</param>
        /// <returns>Configuration object</returns>
        public RuntimeConfiguration UsePackageEntityActivator(IEntityActivator entityActivator)
        {
            _runtimeConfiguration.EntityActivator = entityActivator;
            return _runtimeConfiguration;
        }
    }
}