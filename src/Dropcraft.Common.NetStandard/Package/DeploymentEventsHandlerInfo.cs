namespace Dropcraft.Common.Package
{
    /// <summary>
    /// Describes deployment event handler
    /// </summary>
    public class DeploymentEventsHandlerInfo
    {
        /// <summary>
        /// Package where the handler is defined
        /// </summary>
        public PackageId PackageId { get; }

        /// <summary>
        /// Fully qualified type name
        /// </summary>
        public string ClassName { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="packageId">Origin</param>
        /// <param name="className">Handle class name</param>
        public DeploymentEventsHandlerInfo(PackageId packageId, string className)
        {
            PackageId = packageId;
            ClassName = className;
        }

        /// <summary>
        /// Converts object to string
        /// </summary>
        /// <returns>String</returns>
        public override string ToString()
        {
            return $"{PackageId?.ToString() ?? ""}, {ClassName}";
        }
    }
}