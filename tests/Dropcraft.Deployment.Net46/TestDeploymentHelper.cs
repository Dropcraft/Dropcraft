using System;
using System.IO;
using System.Linq;

namespace Dropcraft.Deployment
{
    public class TestDeploymentHelper : IDisposable
    {
        public string InstallPath { get; set; }
        public string PackagesPath { get; set; }

        public DeploymentConfiguration Configuration { get; set; }

        public TestDeploymentHelper()
        {
            PackagesPath = Path.Combine(Path.GetTempPath(), "DropcraftTests", Path.GetRandomFileName());
            InstallPath = Path.Combine(Path.GetTempPath(), "DropcraftTests", Path.GetRandomFileName());
        }

        public TestDeploymentHelper WithConfiguration(string netFx = "net45")
        {
            Configuration = new DeploymentConfiguration(InstallPath, PackagesPath, netFx);
            return this;
        }

        public TestDeploymentHelper AndNuGetSource()
        {
            Configuration.ForPackages.AddRemoteSource("https://api.nuget.org/v3/index.json");
            return this;
        }

        public bool IsPackageExists(string packageName, string ver)
        {
            var packagePath = Path.Combine(PackagesPath, packageName);
            var verPath = Path.Combine(packagePath, ver);

            return Directory.Exists(packagePath) && Directory.Exists(verPath);
        }

        public bool IsPackageWithAnyVersionExists(string packageNameNoVersion)
        {
            return Directory
                    .EnumerateDirectories(PackagesPath, packageNameNoVersion + "*", SearchOption.TopDirectoryOnly)
                    .Any();
        }

        public void Dispose()
        {
            if (Directory.Exists(InstallPath))
                Directory.Delete(InstallPath, true);

            if (Directory.Exists(PackagesPath))
                Directory.Delete(PackagesPath, true);
        }
    }
}