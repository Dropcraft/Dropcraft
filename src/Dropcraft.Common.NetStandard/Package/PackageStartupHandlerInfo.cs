namespace Dropcraft.Common.Package
{
    /// <summary>
    /// Describes package startup handler
    /// </summary>
    public class PackageStartupHandlerInfo
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
        /// Constructor
        /// </summary>
        /// <param name="packageId">Package ID</param>
        /// <param name="className">Handler class name</param>
        public PackageStartupHandlerInfo(PackageId packageId, string className)
        {
            PackageId = packageId;
            ClassName = className;
        }

        /// <summary>
        /// Converts object to string
        /// </summary>
        /// <returns>string</returns>
        public override string ToString()
        {
            return $"{PackageId?.ToString() ?? ""}, {ClassName}";
        }
    }
}
