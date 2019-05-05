using System;
using System.Collections.Generic;
using System.Text;

namespace IncrementalBackup.Entities
{
    /// <summary>
    /// Information about a file path
    /// </summary>
    class PathInfo
    {
        /// <summary>
        /// Host
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// Root Path
        /// </summary>
        public string RootPath { get; set; }

        /// <summary>
        /// Relative Path
        /// </summary>
        public string RelativePath { get; set; }

        /// <summary>
        /// Date this path was added
        /// </summary>
        public DateTime AddedDateTime { get; set; }

        /// <summary>
        /// Date this path was deleted
        /// </summary>
        public DateTime? DeletedDate { get; set; }
    }
}
