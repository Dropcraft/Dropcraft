using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dropcraft.Common;
using Dropcraft.Deployment.NuGet;

namespace Dropcraft.Deployment
{
    public class TestDeploymentHelper : IDisposable
    {
        public string InstallPath { get; set; }
        public string PackagesPath { get; set; }
        public string Framework { get; set; }

        public bool AllowDowngrades { get; set; }
        public bool UpdatePackages { get; set; }

        public List<string> Sources { get; } = new List<string>();

        public DeploymentConfiguration Configuration { get; set; }

        public TestDeploymentHelper()
        {
            PackagesPath = Path.Combine(Path.GetTempPath(), "DropcraftTests", Path.GetRandomFileName());
            InstallPath = Path.Combine(Path.GetTempPath(), "DropcraftTests", Path.GetRandomFileName());
        }

        public TestDeploymentHelper WithConfiguration(string netFx = "net45")
        {
            Configuration = new DeploymentConfiguration().ForPackages.Cache(PackagesPath);
            Framework = netFx;
            return this;
        }

        public TestDeploymentHelper AndNuGetSource()
        {
            Sources.Add("https://api.nuget.org/v3/index.json");
            Configuration.ForPackages.AddRemoteSource("https://api.nuget.org/v3/index.json");
            return this;
        }

        public IDeploymentEngine CreatEngine()
        {
            return Configuration.CreatEngine(InstallPath, Framework);
        }

        public NuGetEngine CreatNuGetEngine()
        {
            var deploymentEngine = CreatEngine();
            return new NuGetEngine(deploymentEngine.DeploymentContext, PackagesPath, Sources, UpdatePackages, AllowDowngrades);
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