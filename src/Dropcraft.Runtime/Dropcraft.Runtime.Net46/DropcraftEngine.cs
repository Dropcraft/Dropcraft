using Dropcraft.Contracts;

namespace Dropcraft.Runtime
{
    public partial class DropcraftEngine
    {
        protected virtual void InitializePlatformServices()
        {
            if (EntityActivator.Current == null)
                EntityActivator.InitializeEntityActivator(new ReflectionEntityActivator());
        }
    }
}