namespace Dropcraft.Common.Package
{
    /// <summary>
    /// Defines a package file 
    /// </summary>
    public interface IPackageFile
    {
        /// <summary>
        /// Full file path, including file name
        /// </summary>
        string FileName { get; }

        /// <summary>
        /// Determines if two files are the same
        /// </summary>
        /// <param name="packageFile">File to compare with</param>
        /// <returns>Returns true if the files are the same</returns>
        bool IsSameFile(IPackageFile packageFile);
    }
}