using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using IncrementalBackup.Entities;
using Newtonsoft.Json;

namespace IncrementalBackup
{
    class ServiceBackedupFilesRepository : IBackupRepository
    {
        private IBackupServiceClient _backupServiceClient;
        public ServiceBackedupFilesRepository (IBackupServiceClient backupServiceClient)
        {
            _backupServiceClient = backupServiceClient;
        }

        public void BackupFile(string fileId, string filePath)
        {
            var fileBytes = File.ReadAllBytes(filePath);
            _backupServiceClient.WriteFile(fileId, fileBytes).Wait();
        }

        public IDictionary<string, BackupFileInfo> GetFileIndex()
        {
            var result = new Dictionary<string, BackupFileInfo>();
            List<BackupFileInfo> files = null;
            byte[] fileBytes = null; 
            try
            {
                fileBytes = _backupServiceClient.GetFile("_index.json").Result;
            }
            catch
            {
                return result;
            }

            files = JsonConvert.DeserializeObject<List<BackupFileInfo>>(UTF8Encoding.UTF8.GetString(fileBytes));
            foreach (var fileInfo in files)
            {
                result[fileInfo.FileId] = fileInfo;
            }
            return result;
        }

        public void RestoreFile(string fileId, string filePath)
        {
            var fileBytes =  _backupServiceClient.GetFile(fileId).Result;
            File.WriteAllBytes(filePath, fileBytes);
        }

        public void SaveIndex(IDictionary<string, BackupFileInfo> index)
        {
            var jsonIndexBytes = UTF8Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(index.Values));
            _backupServiceClient.WriteFile("_index.json", jsonIndexBytes).Wait();
        }
    }
}
