namespace Dropcraft.Common
{
    /// <summary>
    /// Defines one file for deployment
    /// </summary>
    public class PackageFileInfo
    {
        /// <summary>
        /// Source file path, includes file name
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Target file path, includes file name
        /// </summary>
        public string TargetFilePath { get; set; }

        /// <summary>
        /// Detected file type
        /// </summary>
        public FileType FileType { get; set; }

        /// <summary>
        /// Deployment action
        /// </summary>
        public FileAction Action { get; set; }

        /// <summary>
        /// Is file in conflict state (i.e. target file already exists)? 
        /// </summary>
        public bool Conflict { get; set; }

        /// <summary>
        /// Conflict resolution strategy
        /// </summary>
        public FileConflictResolution ConflictResolution { get; set; }
    }

    /// <summary>
    /// Defines a deployment action 
    /// </summary>
    public enum FileAction
    {
        /// <summary>
        /// Do nothing with the source file
        /// </summary>
        None,

        /// <summary>
        /// Copy source file to the target folder
        /// </summary>
        Copy,

        /// <summary>
        /// Delete file in the target folder (if exists)
        /// </summary>
        Delete
    }

    /// <summary>
    /// Defines type of the file to deploy
    /// </summary>
    public enum FileType
    {
        /// <summary>
        /// Library file
        /// </summary>
        LibFile,

        /// <summary>
        /// Content file
        /// </summary>
        Content,

        /// <summary>
        /// Tool file
        /// </summary>
        Tool,

        /// <summary>
        /// Configuration file from any folder
        /// </summary>
        Configuration,

        /// <summary>
        /// Custom file, will be ignored by the default engine
        /// </summary>
        Custom
    }

    /// <summary>
    /// Defines actions in case of file conflict during deployment
    /// </summary>
    public enum FileConflictResolution
    {
        /// <summary>
        /// Override file in the target folder
        /// </summary>
        Override,
        
        /// <summary>
        /// Keep existing file in the target folder
        /// </summary>
        KeepExisting,

        /// <summary>
        /// Stop deployment
        /// </summary>
        Fail
    }
}
