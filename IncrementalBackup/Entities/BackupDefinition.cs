using System;
using System.Collections.Generic;
using System.Text;

namespace IncrementalBackup.Entities
{
    class BackupDefinition
    {
        public string Destination { get; set; }
        public List<string> SourceDirectories { get; set; }
    }
}
