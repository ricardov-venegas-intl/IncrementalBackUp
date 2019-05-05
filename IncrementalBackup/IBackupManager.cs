using System;
using System.Collections.Generic;
using System.Text;

namespace IncrementalBackup
{
    /// <summary>
    /// Backup Manager Interface
    /// </summary>
    interface IBackupManager
    {
        /// <summary>
        /// Backup Files
        /// </summary>
        void BackupFiles();

        /// <summary>
        /// Restores files
        /// </summary>
        void RestoreFiles(string destinationPath);
    }
}
