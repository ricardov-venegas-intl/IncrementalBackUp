using System.Threading.Tasks;

namespace IncrementalBackup
{
    interface IBackupServiceClient
    {
        Task<byte[]> GetFile(string fileId);
        Task WriteFile(string fileId, byte[] fileContent);
    }
}