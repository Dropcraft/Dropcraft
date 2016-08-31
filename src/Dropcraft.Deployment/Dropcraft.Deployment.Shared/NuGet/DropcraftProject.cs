using System;
using System.Collections.Generic;
using System.Text;
using NuGet.Packaging;
using NuGet.ProjectManagement;

namespace Dropcraft.Deployment.NuGet
{
    internal class DropcraftProject : FolderNuGetProject
    {
        public DropcraftProject(string root) : base(root)
        {
        }

        public DropcraftProject(string root, bool excludeVersion) : base(root, excludeVersion)
        {
        }

        public DropcraftProject(string root, PackagePathResolver packagePathResolver) : base(root, packagePathResolver)
        {
        }
    }
}
