using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dropcraft.Common.Runtime
{
    /// <summary>
    /// Loads configured packages and performs app composition
    /// </summary>
    public interface IRuntimeEngine
    {
        /// <summary>
        /// Current context
        /// </summary>
        RuntimeContext RuntimeContext { get; }

        /// <summary>
        /// Starts execution
        /// </summary>
        /// <returns>Task</returns>
        Task Start();

        /// <summary>
        /// Starts execution for the selected packages and their dependencies
        /// </summary>
        /// <param name="packages">Packages to load</param>
        /// <returns>Task</returns>
        Task Start(ICollection<PackageId> packages);

        /// <summary>
        /// Stops execution. The main purpose is to notify all components about the application shutdown.
        /// </summary>
        void Stop();
    }
}