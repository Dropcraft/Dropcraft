using Dropcraft.Common.Runtime;

namespace Dropcraft.Common.Package
{
    public interface IRuntimeEventsHandler
    {
        void RegisterEventHandlers(RuntimeContext context);
    }

}