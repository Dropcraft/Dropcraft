using System.Collections.Generic;
using Dropcraft.Common;
using NuGet.DependencyResolver;

namespace Dropcraft.Deployment.Workflow
{
    public class ActionablePackage
    {
        public PackageId Id { get; set; }
        public RemoteMatch Match { get; set; }

        public List<ActionablePackage> Dependencies { get; } = new List<ActionablePackage>();
    }
}