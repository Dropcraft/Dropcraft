using Dropcraft.Common.Runtime;

namespace Dropcraft.Common.Package
{
    /// <summary>
    /// Describes runtime events handler
    /// </summary>
    public interface IRuntimeEventsHandler
    {
        /// <summary>
        /// RegisterEventHandlers() is expected to register event handlers with RuntimeContext
        /// </summary>
        /// <param name="context">Current runtime context</param>
        void RegisterEventHandlers(RuntimeContext context);
    }

}