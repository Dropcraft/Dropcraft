
using Dropcraft.Common.Configuration;

namespace Dropcraft.Common
{
    public interface IHandleExtensibilityPoint
    {
        void Initialize(ExtensibilityPointInfo extensibilityPointInfo, RuntimeContext context);

        void RegisterExtension(ExtensionInfo extensionInfo);
    }
}