namespace Dropcraft.Common.Package
{
    /// <summary>
    /// Describes extension
    /// </summary>
    public class ExtensionInfo
    {
        /// <summary>
        /// Extension ID
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Package where extension is defined
        /// </summary>
        public PackageId PackageId { get; }

        /// <summary>
        /// Fully qualified type name
        /// </summary>
        public string ClassName { get; }

        /// <summary>
        /// ID of the corresponding extensibility point
        /// </summary>
        public string ExtensibilityPointId { get; }

        /// <summary>
        /// Extension configuration
        /// </summary>
        public ICustomConfiguration CustomConfiguration { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="packageId">Package ID</param>
        /// <param name="className">Extension class</param>
        /// <param name="extensibilityPointId">Extensibility point ID</param>
        /// <param name="id">Extension ID</param>
        /// <param name="customConfiguration">Extension configuration</param>
        public ExtensionInfo(PackageId packageId, string className, string extensibilityPointId,
                                        string id, ICustomConfiguration customConfiguration)
        {
            PackageId = packageId;
            ClassName = className;
            ExtensibilityPointId = extensibilityPointId;
            Id = id;
            CustomConfiguration = customConfiguration;
        }

        /// <summary>
        /// Converts object to string
        /// </summary>
        /// <returns>string</returns>
        public override string ToString()
        {
            return $"{PackageId?.ToString() ?? ""}, {Id}, {ExtensibilityPointId}, {ClassName}";
        }

    }
}