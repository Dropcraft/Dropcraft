
using Dropcraft.Contracts.Configuration;

namespace Dropcraft.Contracts
{
    public interface IHandleExtensibilityPoint
    {
        void Initialize(ExtensibilityPointInfo extensibilityPointInfo, RuntimeContext context);

        void RegisterExtension(ExtensionInfo extensionInfo);
    }
}