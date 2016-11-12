using System.Collections.Generic;
using Dropcraft.Common;
using NuGet.DependencyResolver;
using NuGet.Frameworks;
using NuGet.LibraryModel;
using NuGet.Versioning;

namespace Dropcraft.Deployment.NuGet
{
    public class ProjectLibraryProvider : IProjectDependencyProvider
    {
        private readonly List<PackageId> _packages;

        public ProjectLibraryProvider(List<PackageId> packages)
        {
            _packages = packages;
        }

        public bool SupportsType(LibraryDependencyTarget libraryTypeFlag)
        {
            return libraryTypeFlag == LibraryDependencyTarget.Project;
        }

        public Library GetLibrary(LibraryRange libraryRange, NuGetFramework targetFramework)
        {
            return GetLibrary(libraryRange, targetFramework, null);
        }

        public Library GetLibrary(LibraryRange libraryRange, NuGetFramework targetFramework, string rootPath)
        {
            var dependencies = new List<LibraryDependency>();

            foreach (var package in _packages)
            {
                var lib = new LibraryDependency
                {
                    LibraryRange =
                        new LibraryRange(package.Id, VersionRange.Parse(package.Version),
                            LibraryDependencyTarget.Package)
                };

                dependencies.Add(lib);
            }

            return new Library
            {
                LibraryRange = libraryRange,
                Identity = new LibraryIdentity
                {
                    Name = libraryRange.Name,
                    Version = NuGetVersion.Parse("1.0.0"),
                    Type = LibraryType.Project,
                },
                Dependencies = dependencies,
                Resolved = true
            };
        }
    }
}