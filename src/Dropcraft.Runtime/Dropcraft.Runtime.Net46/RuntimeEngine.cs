using Dropcraft.Common;

namespace Dropcraft.Runtime
{
    public partial class RuntimeEngine
    {
        protected virtual void InitializePlatformServices()
        {
            if (EntityActivator.Current == null)
                EntityActivator.InitializeEntityActivator(new ReflectionEntityActivator());
        }
    }
}