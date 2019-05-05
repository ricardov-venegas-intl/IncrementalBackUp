using IncrementalBackup.Entities;
using System.Collections.Generic;

namespace IncrementalBackup
{
    /// <summary>
    /// Backup repository
    /// </summary>
    interface IBackupRepository
    {
        /// <summary>
        /// Get the files in the location
        /// </summary>
        /// <returns></returns>
        IDictionary<string, BackupFileInfo> GetFileIndex();

        /// <summary>
        /// Saves the Backup Index
        /// </summary>
        /// <param name="index"></param>
        void SaveIndex(IDictionary<string, BackupFileInfo> index);

        /// <summary>
        /// Backups a file
        /// </summary>
        /// <param name="fileId"></param>
        /// <param name="filePath"></param>
        void BackupFile(string fileId, string filePath);

        /// <summary>
        /// Restores a File a file
        /// </summary>
        /// <param name="fileId"></param>
        /// <param name="filePath"></param>
        void RestoreFile(string fileId, string filePath);

    }
}
