using Dropcraft.Common.Runtime;

namespace Dropcraft.Common.Package
{
    public interface IExtensibilityPointHandler
    {
        void Initialize(ExtensibilityPointInfo extensibilityPointInfo, RuntimeContext context);

        void RegisterExtension(ExtensionInfo extensionInfo);
    }
}