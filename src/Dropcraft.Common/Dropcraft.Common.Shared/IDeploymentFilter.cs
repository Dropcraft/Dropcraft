using Dropcraft.Common.Package;

namespace Dropcraft.Common
{
    public interface IDeploymentFilter
    {
        void Filter(InstallablePackageInfo installablePackageInfo);
    }
}
