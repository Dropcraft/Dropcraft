using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dropcraft.Common;
using Dropcraft.Common.Logging;

namespace Dropcraft.Deployment.Workflow
{
    public class FileTransaction : IDisposable
    {
        private readonly string _backupFolder;
        private readonly List<string> _installedFile = new List<string>();
        private readonly List<string> _createdFolder = new List<string>();
        private readonly List<FileRecord> _deletedFiles = new List<FileRecord>();
        private static readonly ILog Logger = LogProvider.For<FileTransaction>();

        public FileTransaction()
        {
            _backupFolder = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            if (!Directory.Exists(_backupFolder))
            {
                Directory.CreateDirectory(_backupFolder);
            }
        }

        public void CreateFolder(string folder)
        {
            if (!Directory.Exists(folder))
            {
                _createdFolder.Add(folder);
                Logger.Trace($"Creating folder {folder}");
                Directory.CreateDirectory(folder);
            }
        }

        public void DeleteFile(string fileName)
        {
            var targetPath = Path.Combine(_backupFolder, Path.GetFileName(fileName));
            File.Copy(fileName, targetPath);
            _deletedFiles.Add(new FileRecord {OriginalFile = fileName, BackupFile = targetPath});

            Logger.Trace($"Deleting {fileName}");
            File.Delete(fileName);

            var folder = Path.GetDirectoryName(fileName);
            if (!Directory.EnumerateFileSystemEntries(folder).Any())
            {
                Logger.Trace($"Removing empty folder {folder}");
                Directory.Delete(folder);
            }
        }

        public void InstallFile(PackageFileInfo fileInfo)
        {
            Logger.Trace($"Installing file {fileInfo.FileName} to {fileInfo.TargetFileName}");
            if (File.Exists(fileInfo.TargetFileName))
            {
                Logger.Trace($"Conflict between {fileInfo.FileName} and {fileInfo.TargetFileName}");
                switch (fileInfo.ConflictResolution)
                {
                    case FileConflictResolution.KeepExisting:
                        Logger.Trace("Conflict resolved: keep existing file");
                        return;
                    case FileConflictResolution.Fail:
                        Logger.Trace("Conflict resolved: fail");
                        throw new IOException($"Deployment stopped because of conflict between {fileInfo.FileName} and {fileInfo.TargetFileName}");
                }

                Logger.Trace("Conflict resolved: override file");
                DeleteFile(fileInfo.TargetFileName);
            }
            else
            {
                var folder = Path.GetDirectoryName(fileInfo.TargetFileName);
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);
            }

            File.Copy(fileInfo.FileName, fileInfo.TargetFileName);
            _installedFile.Add(fileInfo.TargetFileName);
            Logger.Trace($"File {fileInfo.TargetFileName} copied");
        }

        public void Commit()
        {
            _installedFile.Clear();
            _deletedFiles.Clear();
            _createdFolder.Clear();
        }

        public void Dispose()
        {
            Rollback();

            if (Directory.Exists(_backupFolder))
            {
                Directory.Delete(_backupFolder, true);
            }
        }

        private void Rollback()
        {
            if (_installedFile.Count == 0 && _deletedFiles.Count == 0 && _createdFolder.Count == 0)
            {
                return;
            }

            Logger.Trace("Rolling back changes");

            foreach (var file in _installedFile)
            {
                if (File.Exists(file))
                {
                    Logger.Trace($"Rolling back file copy for {file}");
                    File.Delete(file);
                }
            }

            foreach (var fileRecord in _deletedFiles)
            {
                Logger.Trace($"Rolling back file deletion for {fileRecord.OriginalFile}");

                if (!File.Exists(fileRecord.BackupFile))
                {
                    Logger.Trace($"Backup file not found {fileRecord.BackupFile}");
                    continue;
                }

                var folder = Path.GetDirectoryName(fileRecord.OriginalFile);
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }

                File.Copy(fileRecord.BackupFile, fileRecord.OriginalFile, true);
            }

            foreach (var folder in _createdFolder)
            {
                if (!Directory.EnumerateFileSystemEntries(folder).Any())
                {
                    Logger.Trace($"Rolling back folder creation {folder}");
                    Directory.Delete(folder);
                }
            }

            
            Commit();
        }
    }
    class FileRecord
    {
        public string OriginalFile { get; set; }
        public string BackupFile { get; set; }
    }
}