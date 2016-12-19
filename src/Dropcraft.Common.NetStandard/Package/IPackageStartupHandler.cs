using Dropcraft.Common.Runtime;

namespace Dropcraft.Common.Package
{
    public interface IPackageStartupHandler
    {
        void Start(RuntimeContext runtimeContext);
    }
}