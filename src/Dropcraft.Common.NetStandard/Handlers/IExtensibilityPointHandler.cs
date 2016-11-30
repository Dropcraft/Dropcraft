
using Dropcraft.Common.Configuration;

namespace Dropcraft.Common.Handlers
{
    public interface IExtensibilityPointHandler
    {
        void Initialize(ExtensibilityPointInfo extensibilityPointInfo, RuntimeContext context);

        void RegisterExtension(ExtensionInfo extensionInfo);
    }
}