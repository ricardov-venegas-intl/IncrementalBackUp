using IncrementalBackup.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace IncrementalBackup
{
    /// <summary>
    /// Repository of files to backup
    /// </summary>
    interface IFileSourceRepository
    {
        /// <summary>
        /// Get the files in the location
        /// </summary>
        /// <returns></returns>
        IDictionary<string, BackupFileInfo> GetFileIndex();
    }
}
