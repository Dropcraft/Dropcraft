using System;
using System.IO;

namespace Dropcraft.Deployment
{
    public class TestDeploymentHelper : IDisposable
    {
        public string InstallPath { get; set; }
        public string PackagesPath { get; set; }

        public TestDeploymentHelper()
        {
            PackagesPath = Path.Combine(Path.GetTempPath(), "DropcraftTests", Path.GetRandomFileName());
            InstallPath = Path.Combine(Path.GetTempPath(), "DropcraftTests", Path.GetRandomFileName());
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