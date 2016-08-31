using System.Collections.Generic;

namespace Dropcraft.Deployment.Options
{
    public class DeploymentOptions
    {
        public string Path { get; set; }

        public List<DeploymentPackage> Packages { get; } = new List<DeploymentPackage>();
    }

    public class DeploymentPackage
    {
        public string Id { get; set; }
        public string Version { get; set; }
    }

    public class InstallOptions : DeploymentOptions
    {
        public List<string> Sources { get; } = new List<string>();
    }

    public class UpdateOptions : DeploymentOptions
    {
        public List<string> Sources { get; } = new List<string>();
    }

    public class UninstallOptions : DeploymentOptions
    {
    }
}
