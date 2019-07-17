using System.Collections.Generic;
using System.IO;
using IncrementalBackup.Entities;
using Newtonsoft.Json;

namespace IncrementalBackup
{
    class FileSystemBackedupFilesRepository : IBackupRepository
    {
        DirectoryInfo _directoryInfo;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="directoryInfo"></param>
        public FileSystemBackedupFilesRepository(DirectoryInfo directoryInfo)
        {
            _directoryInfo = directoryInfo;
        }


        /// <summary>
        /// Get the files in the location
        /// </summary>
        /// <returns></returns>
        public IDictionary<string, BackupFileInfo> GetFileIndex()
        {
            var result = new Dictionary<string, BackupFileInfo>();
            var pathInfo = Path.Join(_directoryInfo.FullName, "_index.json");
            var reader = JsonSerializer.Create();
            List<BackupFileInfo> files = null;
            if (File.Exists(pathInfo) == false)
            {
                return result;
            }
            using (JsonTextReader fileReader = new JsonTextReader(File.OpenText(pathInfo)))
            {
                files = (List<BackupFileInfo>)reader.Deserialize(fileReader, typeof(List<BackupFileInfo>));
            }
            foreach (var fileInfo in files)
            {
                result[fileInfo.FileId] = fileInfo;
            }
            return result;
        }

        /// <summary>
        /// Saves the index of the files in the backup repository
        /// </summary>
        /// <param name="index"></param>
        public void SaveIndex(IDictionary<string, BackupFileInfo> index)
        {
            var pathInfo = Path.Join(_directoryInfo.FullName, "_index.json");
            JsonSerializer serializer = new JsonSerializer();
            using (var streamWriter = File.CreateText(pathInfo))
            {
                using (JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter))
                {
                    serializer.Serialize(jsonWriter, index.Values);
                }
            }
        }

        /// <summary>
        /// Saves a file into the backup
        /// </summary>
        /// <param name="fileId"></param>
        /// <param name="sourcePath"></param>
        public void BackupFile(string fileId, string sourcePath)
        {
            var destinationPath = Path.Join(_directoryInfo.FullName, fileId + ".bin");
            if (File.Exists(destinationPath) == false)
            {
                File.Copy(sourcePath, destinationPath);
            }
        }


        /// <summary>
        /// Restores a file from the backup
        /// </summary>
        /// <param name="fileId"></param>
        /// <param name="destinationPath"></param>
        public void RestoreFile(string fileId, string destinationPath)
        {
            var sourcePath = Path.Join(_directoryInfo.FullName, fileId + ".bin");
            var destinationDirectory = Path.GetDirectoryName(destinationPath);
            if (Directory.Exists(destinationDirectory) == false)
            {
                Directory.CreateDirectory(destinationDirectory);
            }
            File.Copy(sourcePath, destinationPath, true);
        }

    }
}
