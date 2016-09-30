
using Dropcraft.Common.Configuration;

namespace Dropcraft.Common.Handler
{
    public interface IExtensibilityPointHandler
    {
        void Initialize(ExtensibilityPointInfo extensibilityPointInfo, IRuntimeContext context);

        void RegisterExtension(ExtensionInfo extensionInfo);
    }
}