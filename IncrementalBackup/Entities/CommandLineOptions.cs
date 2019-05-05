using System;
using System.Collections.Generic;
using System.Text;
using CommandLine;
using CommandLine.Attributes;
using CommandLine.Attributes.Advanced;

namespace IncrementalBackup.Entities
{
    /// <summary>
    /// Defines the commands and parameters accepted by the command line.
    /// </summary>
    class CommandLineOptions
    {
        /// <summary>
        /// Action to execute (backup/restore)
        /// </summary>
        [ActionArgument]
        public Commands Action { get; set; }

        /// <summary>
        /// Path to the file containing the backup plan definition
        /// </summary>
        [CommonArgument]
        [RequiredArgument(0, "definitionFile", "Path to the file containing the backup plan definition")]
        public string DefinitionFile { get; set; }

        /// <summary>
        /// Destination folder when performing Restore
        /// </summary>
        [OptionalArgument(null, "destinationDirectory", "Destination folder when performing Restore")]
        [ArgumentGroup(nameof(Commands.Restore))]
        public string DestinationDirectory { get; set; }

        /// <summary>
        /// Name of the computer where the backup was used when performing Restore
        /// </summary>
        [OptionalArgument(null, "host", "Name of the computer where the backup was used when performing Restore")]
        [ArgumentGroup(nameof(Commands.Restore))]
        public string Host { get; set; }

    }

    /// <summary>
    /// Commands to execute.
    /// </summary>
    public enum Commands
    {
        Backup,
        Restore
    }
}
