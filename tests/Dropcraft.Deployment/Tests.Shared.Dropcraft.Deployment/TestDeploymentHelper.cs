using System;
using System.IO;
using System.Linq;
using Dropcraft.Deployment;

namespace Tests.Dropcraft.Deployment
{
    internal class TestDeploymentHelper : IDisposable
    {
        public string InstallPath { get; set; }
        public string PackagesPath { get; set; }
        public IDeploymentEngine Engine { get; set; }

        public TestDeploymentHelper()
        {
            PackagesPath = Path.Combine(Path.GetTempPath(), "DropcraftTests", Path.GetRandomFileName());
            InstallPath = Path.Combine(Path.GetTempPath(), "DropcraftTests", Path.GetRandomFileName());

        }

        public TestDeploymentHelper WithDefaultEngine()
        {
            Engine = new DeploymentConfiguration(InstallPath, PackagesPath)
                .AddPackageSource("https://api.nuget.org/v3/index.json")
                .CreatEngine();

            return this;
        }

        public bool IsPackageExists(string packageName)
        {
            return Directory.Exists(Path.Combine(PackagesPath, packageName));
        }

        public bool IsSimilarPackageExists(string packageNameNoVersion)
        {
            return Directory
                    .EnumerateDirectories(PackagesPath, packageNameNoVersion+"*", SearchOption.TopDirectoryOnly)
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