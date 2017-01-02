using Dropcraft.Common.Runtime;

namespace Dropcraft.Common.Package
{
    /// <summary>
    /// Describes package startup handler
    /// </summary>
    public interface IPackageStartupHandler
    {
        /// <summary>
        /// Handles startup of the package
        /// </summary>
        /// <param name="runtimeContext">Current runtime context</param>
        void Start(RuntimeContext runtimeContext);
    }
}