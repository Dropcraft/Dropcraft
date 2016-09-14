using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dropcraft.Common.Diagnostics;
using Dropcraft.Common.Package;
using NuGet.Frameworks;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.ProjectManagement;
using NuGet.Protocol.Core.Types;

namespace Dropcraft.Deployment.NuGet
{
    internal class DropcraftProject : FolderNuGetProject
    {
        public FileConflictResolution DefaultConflictResolution { get; set; }

        private readonly FrameworkReducer _reducer = new FrameworkReducer();

        public NuGetFramework CurrentFramework { get; set; }

        public List<PackageIdentity> RecentPackages { get; } = new List<PackageIdentity>();

        public DropcraftProject(string root) : base(root)
        {
        }

        public DropcraftProject(string root, bool excludeVersion) : base(root, excludeVersion)
        {
        }

        public DropcraftProject(string root, PackagePathResolver packagePathResolver) : base(root, packagePathResolver)
        {
        }

        public void CleanRecentPackages()
        {
            RecentPackages.Clear();
        }

        public List<InstallablePackage> ProcessRecentPackages()
        {
            var installedPackages = new List<InstallablePackage>();

            foreach (var package in RecentPackages)
            {
                var installablePackage = new InstallablePackage(package.Id, package.Version.ToString(), package.Version.IsPrerelease, package.Version);

                var installedPath = GetInstalledPath(package);
                var packageFilePath = GetInstalledPackageFilePath(package);
                using (var archiveReader = new PackageArchiveReader(packageFilePath))
                {
                    AddFiles(installablePackage, installedPath, archiveReader.GetReferenceItems().ToList(), FileType.LibFile, FileAction.Copy);
                    AddFiles(installablePackage, installedPath, archiveReader.GetContentItems().ToList(), FileType.Content, FileAction.Copy);
                    AddFiles(installablePackage, installedPath, archiveReader.GetToolItems().ToList(), FileType.Tool, FileAction.None);
                }

                installedPackages.Add(installablePackage);
            }

            return installedPackages;
        }

        private void AddFiles(InstallablePackage package, string installedPath, List<FrameworkSpecificGroup> groups, FileType fileType, FileAction action)
        {
            var referenceGroup = GetMostCompatibleGroup(_reducer, CurrentFramework, groups);
            if (referenceGroup == null) return;

            foreach (var itemPath in referenceGroup.Items)
            {
                var fileInfo = new InstallableFileInfo
                {
                    FilePath = Path.Combine(installedPath, itemPath),
                    FileType = fileType,
                    Action = action,
                    ConflictResolution = DefaultConflictResolution
                };

                package.InstallableFiles.Add(fileInfo);
            }
        }

        public override Task<bool> InstallPackageAsync(PackageIdentity packageIdentity, DownloadResourceResult downloadResourceResult,
            INuGetProjectContext nuGetProjectContext, CancellationToken token)
        {
            Trace.Current.Verbose($"Installing package {packageIdentity.Id} {packageIdentity.Version}");

            RecentPackages.Add(packageIdentity);
            return base.InstallPackageAsync(packageIdentity, downloadResourceResult, nuGetProjectContext, token);
        }

        // The following methods are originally from the internal MSBuildNuGetProjectSystemUtility class
        #region MSBuildNuGetProjectSystemUtility  

        private static FrameworkSpecificGroup GetMostCompatibleGroup(FrameworkReducer reducer, NuGetFramework projectTargetFramework,
            ICollection<FrameworkSpecificGroup> itemGroups)
        {
            NuGetFramework mostCompatibleFramework
                = reducer.GetNearest(projectTargetFramework, itemGroups.Select(i => i.TargetFramework));
            if (mostCompatibleFramework != null)
            {
                FrameworkSpecificGroup mostCompatibleGroup = itemGroups
                    .FirstOrDefault(i => i.TargetFramework.Equals(mostCompatibleFramework));

                if (IsValid(mostCompatibleGroup))
                {
                    // Normalize() is called outside GetMostCompatibleGroup() in MSBuildNuGetProjectSystemUtility but I combined it
                    return Normalize(mostCompatibleGroup);
                }
            }

            return null;
        }


        private static bool IsValid(FrameworkSpecificGroup frameworkSpecificGroup)
        {
            if (frameworkSpecificGroup != null)
            {
                return (frameworkSpecificGroup.HasEmptyFolder
                     || frameworkSpecificGroup.Items.Any()
                     || !frameworkSpecificGroup.TargetFramework.Equals(NuGetFramework.AnyFramework));
            }

            return false;
        }

        private static FrameworkSpecificGroup Normalize(FrameworkSpecificGroup group)
        {
            // Default to returning the same group
            FrameworkSpecificGroup result = group;

            // If the group is null or it does not contain any items besides _._ then this is a no-op.
            // If it does have items create a new normalized group to replace it with.
            if (group?.Items.Any() == true)
            {
                // Filter out invalid files
                IEnumerable<string> normalizedItems = GetValidPackageItems(group.Items)
                    .Select(PathUtility.ReplaceAltDirSeparatorWithDirSeparator);

                // Create a new group
                result = new FrameworkSpecificGroup(group.TargetFramework, normalizedItems);
            }

            return result;
        }

        private static IEnumerable<string> GetValidPackageItems(IEnumerable<string> items)
        {
            // Assume nupkg and nuspec as the save mode for identifying valid package files
            return items?.Where(i => PackageHelper.IsPackageFile(i, PackageSaveMode.Defaultv3)) ?? Enumerable.Empty<string>();
        }

        #endregion MSBuildNuGetProjectSystemUtility

    }
}
