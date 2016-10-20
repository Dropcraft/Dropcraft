
using Dropcraft.Common.Configuration;

namespace Dropcraft.Common.Handler
{
    public interface IExtensibilityPointHandler
    {
        void Initialize(ExtensibilityPointInfo extensibilityPointInfo, RuntimeContext context);

        void RegisterExtension(ExtensionInfo extensionInfo);
    }
}