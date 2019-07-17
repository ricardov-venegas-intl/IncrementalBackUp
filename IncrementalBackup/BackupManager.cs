using IncrementalBackup.Entities;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace IncrementalBackup
{
    /// <summary>
    /// Backup Manager
    /// Performs backup and restore operations on repositories
    /// </summary>
    class BackupManager : IBackupManager
    {
        ILocalFileSystemRepository _sourceFilesRepository;
        IBackupRepository _backedupFilesRepository;
        string _hostName;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sourceRepository"></param>
        /// <param name="destinationRepository"></param>
        /// <param name="hostName"></param>
        public BackupManager(ILocalFileSystemRepository sourceRepository, IBackupRepository destinationRepository, string hostName)
        {
            _sourceFilesRepository = sourceRepository;
            _backedupFilesRepository = destinationRepository;
            _hostName = hostName;
        }

        /// <summary>
        /// Backups files not found in the backup
        /// </summary>
        public void BackupFiles()
        {
            var sourceIndex = _sourceFilesRepository.GetFileIndex();
            var destinationIndex = _backedupFilesRepository.GetFileIndex();
            HashSet<string> newFiles = new HashSet<string>();
            HashSet<string> foundFiles = new HashSet<string>();
            foreach (var fileId in sourceIndex.Keys)
            {
                if (destinationIndex.ContainsKey(fileId) == true)
                {
                    foundFiles.Add(fileId);
                }
                else
                {
                    newFiles.Add(fileId);
                }
            }
            CopyNewFilesToBackup(newFiles, sourceIndex, destinationIndex);
            UpdateDestinationIndexWithNewPaths(foundFiles, sourceIndex, destinationIndex);
            _backedupFilesRepository.SaveIndex(destinationIndex);
        }

        /// <summary>
        /// Restores files from the backup repository
        /// </summary>
        /// <param name="destinationPath"></param>
        public void RestoreFiles(string destinationPath)
        {
            var destinationIndex = _backedupFilesRepository.GetFileIndex();
            foreach (var file in destinationIndex.Values)
            {
                foreach (var filePath in file.Paths.Where(p => string.Equals(p.Host, _hostName, StringComparison.InvariantCultureIgnoreCase)))
                {
                    string destinationFilePath = Path.Join(destinationPath, filePath.RelativePath);
                    _backedupFilesRepository.RestoreFile(file.FileId, destinationFilePath);
                }
            }
        }

        private void CopyNewFilesToBackup(HashSet<string> newFiles, IDictionary<string, BackupFileInfo> sourceIndex, IDictionary<string, BackupFileInfo> destinationIndex)
        {
            foreach (var fileId in newFiles)
            {
                var source = sourceIndex[fileId].Paths[0];
                var sourcePath = Path.Join(source.RootPath, source.RelativePath);
                _backedupFilesRepository.BackupFile(fileId, sourcePath);
                destinationIndex[fileId] = sourceIndex[fileId];
            }
        }

        private void UpdateDestinationIndexWithNewPaths(HashSet<string> foundFiles, IDictionary<string, BackupFileInfo> sourceIndex, IDictionary<string, BackupFileInfo> destinationIndex)
        {
            foreach (var fileId in foundFiles)
            {
                var destinationFileInfo = destinationIndex[fileId];
                var sourceFileInfo = sourceIndex[fileId];
                foreach (var newPath in sourceFileInfo.Paths)
                {
                    if (destinationFileInfo.Paths.Any(p => string.Equals(p.Host, newPath.Host, StringComparison.InvariantCultureIgnoreCase)
                                                            && string.Equals(p.RootPath, newPath.RootPath, StringComparison.InvariantCultureIgnoreCase)
                                                            && string.Equals(p.RelativePath, newPath.RelativePath, StringComparison.InvariantCultureIgnoreCase)) == false)
                    {
                        destinationFileInfo.Paths.Add(newPath);
                    }
                }
            }
        }

        private void UpdateDestinationIndexWithDeletedPaths(IDictionary<string, BackupFileInfo> sourceIndex, IDictionary<string, BackupFileInfo> destinationIndex, string hostName)
        {
            foreach (var destinationFileInfo in destinationIndex.Values)
            {
                BackupFileInfo sourceFileInfo = null;
                if (sourceIndex.TryGetValue(destinationFileInfo.FileId, out sourceFileInfo) == true)
                {
                    // If the destination exists in source, verify all paths exist
                    foreach (var destinationPath in destinationFileInfo.Paths.Where(p => string.Equals(p.Host, hostName, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        if (sourceFileInfo.Paths.Any(sourcePath => string.Equals(sourcePath.Host, destinationPath.Host, StringComparison.InvariantCultureIgnoreCase)
                                                            && string.Equals(sourcePath.RootPath, destinationPath.RootPath, StringComparison.InvariantCultureIgnoreCase)
                                                            && string.Equals(sourcePath.RelativePath, destinationPath.RelativePath)) == false)
                        {
                            destinationPath.DeletedDate = DateTime.UtcNow;
                        }
                    }
                }
                else
                {
                    // Mark as deleted if the file is not found
                    foreach (var destinationPath in destinationFileInfo.Paths.Where(p => string.Equals(p.Host, hostName, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        destinationPath.DeletedDate = DateTime.UtcNow;
                    }
                }
            }
        }
    }
}
