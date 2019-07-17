using System;
using System.IO;
using Newtonsoft.Json;
using IncrementalBackup.Entities;
using CommandLine;
using System.Linq;

namespace IncrementalBackup
{
    /// <summary>
    /// Program entry point
    /// </summary>
    class Program
    {
        /// <summary>
        /// Program entry point
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            CommandLineOptions commandLineOptions;
            if (Parser.TryParse(args, out commandLineOptions) == false)
            {
                return;
            }
            if (commandLineOptions.Host == null)
            {
                commandLineOptions.Host = Environment.MachineName;
            }
            var backupDefinition = JsonConvert.DeserializeObject<BackupDefinition>(File.ReadAllText(commandLineOptions.DefinitionFile));
            if (commandLineOptions.Action == Commands.Backup)
            {
                var sourceDirectories = backupDefinition.SourceDirectories.Select(sd => new DirectoryInfo(sd)).ToArray();
                ILocalFileSystemRepository sourceRepository = new LocalFileSystemRepository(sourceDirectories, new FSHA256Hasher());
                //IBackupRepository destinationRepository = new FileSystemBackedupFilesRepository(new DirectoryInfo(backupDefinition.Destination));
                IBackupRepository backupRepository = new ServiceBackedupFilesRepository(new BackupServiceClient(backupDefinition.Destination));
                IBackupManager backupManager = new BackupManager(sourceRepository, backupRepository, commandLineOptions.Host);
                backupManager.BackupFiles();
            }
            else if (commandLineOptions.Action == Commands.Restore)
            {
                var sourceDirectories = backupDefinition.SourceDirectories.Select(sd => new DirectoryInfo(sd)).ToArray();
                ILocalFileSystemRepository sourceRepository = new LocalFileSystemRepository(sourceDirectories, new FSHA256Hasher());
                IBackupRepository backupRepository = new ServiceBackedupFilesRepository(new BackupServiceClient(backupDefinition.Destination));
                IBackupManager backupManager = new BackupManager(sourceRepository, backupRepository, commandLineOptions.Host);
                backupManager.RestoreFiles(commandLineOptions.DestinationDirectory);
            }
        }
    }
}
