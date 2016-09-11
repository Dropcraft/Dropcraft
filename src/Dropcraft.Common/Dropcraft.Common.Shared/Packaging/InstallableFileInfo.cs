namespace Dropcraft.Common.Package
{
    public class InstallableFileInfo
    {
        public string FilePath { get; set; }
        public string TargetFilePath { get; set; }
        public FileType FileType { get; set; }
        public FileAction Action { get; set; }
    }

    public enum FileAction
    {
        None,
        Copy,
        Delete
    }

    public enum FileType
    {
        Assembly,
        Content,
        Tool,
        Custom
    }
}
