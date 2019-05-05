using System;
using System.Collections.Generic;
using System.Text;


namespace IncrementalBackup.Entities
{
    /// <summary>
    /// Information about a backed up file
    /// </summary>
    class BackupFileInfo
    {
        /// <summary>
        /// Id of the file: SHA256
        /// </summary>
        public string FileId { get; set; }

        /// <summary>
        /// Paths associated to this file
        /// </summary>
        public List<PathInfo> Paths { get; set; }

    }
}
