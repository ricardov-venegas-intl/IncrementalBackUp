using IncrementalBackup.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace IncrementalBackup
{
    /// <summary>
    /// Source Files Repository
    /// </summary>
    class LocalFileSystemRepository : ILocalFileSystemRepository
    {
        DirectoryInfo[] _directoriesInfo;
        IFileHasher _fileHasher;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="directoriesInfo"></param>
        /// <param name="fileHasher"></param>
        public LocalFileSystemRepository (DirectoryInfo[] directoriesInfo, IFileHasher fileHasher)
        {
            _directoriesInfo = directoriesInfo;
            _fileHasher = fileHasher;
        }


        public IDictionary<string,BackupFileInfo> GetFileIndex()
        {
            IDictionary<string, BackupFileInfo> result = new Dictionary<string, BackupFileInfo>();
            foreach (var directoryInfo in _directoriesInfo)
            {
                string host = Environment.MachineName;
                string rootPath = directoryInfo.FullName;
                int rootLength = rootPath.Length + 1;
                foreach (var file in directoryInfo.GetFiles("*", SearchOption.AllDirectories))
                {
                    string relativePath = file.FullName.Substring(rootLength);
                    PathInfo filePathInfo = new PathInfo
                    {
                        Host = host,
                        RootPath = rootPath,
                        RelativePath = relativePath,
                        AddedDateTime = DateTime.UtcNow,
                        DeletedDate = null
                    };
                    BackupFileInfo backupFileInfo = new BackupFileInfo();
                    using (var fileStream = file.OpenRead())
                    {
                        backupFileInfo.FileId = _fileHasher.CalculateHash(fileStream);
                    }
                    if (result.ContainsKey(backupFileInfo.FileId))
                    {
                        backupFileInfo = result[backupFileInfo.FileId];
                    }
                    else
                    {
                        backupFileInfo.Paths = new List<PathInfo>();
                        result[backupFileInfo.FileId] = backupFileInfo;
                    }
                    backupFileInfo.Paths.Add(filePathInfo);
                }
            }
            return result;
        }
    }
}
