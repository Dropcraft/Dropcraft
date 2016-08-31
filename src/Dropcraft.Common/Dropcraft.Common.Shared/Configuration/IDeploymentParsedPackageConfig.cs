namespace Dropcraft.Common.Configuration
{
    public interface IDeploymentParsedPackageConfig
    {

    }
}

/*

    Deployment:
        - Re-configuration
        - Update system
        - Copy files
    
    Pipeline:
        - Install packages in temp location, resolve dependencies => all packages unpacked, packages catalog created
        - Read package cfg to get IHandlePackageDeployment objects
        - execute IHandlePackageDeployment.AssemblyFilter(file list with action (copy, none) and target location)
        - execute DeploymentContext.AssemblyFilter(file list with action (copy, none) and target location)
        - copy to target folder
        - repeat for all packages
        - combine packages catalog
        - IHandlePackageDeployment.Configure(package)
        - DeploymentContext.Configure(package)
        - IHandlePackageDeployment.FinalizeDeployment(package)

    update??
    uninstall??
 */
