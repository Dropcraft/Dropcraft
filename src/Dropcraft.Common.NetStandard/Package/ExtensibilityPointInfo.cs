namespace Dropcraft.Common.Package
{
    /// <summary>
    /// Describes extensibility point
    /// </summary>
    public class ExtensibilityPointInfo
    {
        /// <summary>
        /// Package ID
        /// </summary>
        public PackageId PackageId { get; }

        /// <summary>
        /// Fully qualified type name
        /// </summary>
        public string ClassName { get; }

        /// <summary>
        /// Extensibility point ID. Used to reference extensibility point by extensions
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Extensibility point activation mode
        /// </summary>
        public EntityActivationMode ActivationMode { get; }

        /// <summary>
        /// Extensibility point configuration
        /// </summary>
        public ICustomConfiguration CustomConfiguration { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="packageId">Package ID</param>
        /// <param name="className">Extensibility point handler class name</param>
        /// <param name="id">Extensibility point ID</param>
        /// <param name="activationMode">Extensibility point activation mode</param>
        /// <param name="customConfiguration">Extensibility point configuration</param>
        public ExtensibilityPointInfo(PackageId packageId, string className, string id, 
                                        EntityActivationMode activationMode, ICustomConfiguration customConfiguration)
        {
            PackageId = packageId;
            ClassName = className;
            Id = id;
            ActivationMode = activationMode;
            CustomConfiguration = customConfiguration;
        }

        /// <summary>
        /// Converts object to string
        /// </summary>
        /// <returns>string</returns>
        public override string ToString()
        {
            return $"{PackageId?.ToString() ?? ""}, {Id}, {ClassName}, {ActivationMode}";
        }
    }
}