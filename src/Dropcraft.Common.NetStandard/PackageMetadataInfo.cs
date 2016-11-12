namespace Dropcraft.Common
{
    public class PackageMetadataInfo
    {
        /// <summary>
        /// Title provides human-friendly package title 
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Authors provides a comma-separated list of authors of the package
        /// </summary>
        public string Authors { get; }

        /// <summary>
        /// Description provides a long description of the package
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// ProjectUrl provides a URL for the home page of the package
        /// </summary>
        public string ProjectUrl { get; }

        /// <summary>
        /// IconUrl provides a URL for the image to use as the icon for the package
        /// </summary>
        public string IconUrl { get; }

        /// <summary>
        /// LicenseUrl provides a link to the license that the package is under
        /// </summary>
        public string LicenseUrl { get; }

        /// <summary>
        /// Provides copyright details for the package
        /// </summary>
        public string Copyright { get; }

        public PackageMetadataInfo(string title, string authors, string description, string projectUrl, string iconUrl, string licenseUrl, string copyright)
        {
            Title = title;
            Authors = authors;
            Description = description;
            ProjectUrl = projectUrl;
            IconUrl = iconUrl;
            LicenseUrl = licenseUrl;
            Copyright = copyright;
        }
    }
}
