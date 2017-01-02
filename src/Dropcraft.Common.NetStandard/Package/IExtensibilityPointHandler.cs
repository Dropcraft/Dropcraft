using Dropcraft.Common.Runtime;

namespace Dropcraft.Common.Package
{
    /// <summary>
    /// Defines handler for registered extensibility point
    /// </summary>
    public interface IExtensibilityPointHandler
    {
        /// <summary>
        /// Initializes extensibility point
        /// </summary>
        /// <param name="extensibilityPointInfo">Information about extensibility point</param>
        /// <param name="context">Current RuntimeContext</param>
        void Initialize(ExtensibilityPointInfo extensibilityPointInfo, RuntimeContext context);

        /// <summary>
        /// Registers new extension
        /// </summary>
        /// <param name="extensionInfo">Information about the extension to register</param>
        void RegisterExtension(ExtensionInfo extensionInfo);
    }
}