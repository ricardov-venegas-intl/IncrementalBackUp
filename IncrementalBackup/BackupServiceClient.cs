using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace IncrementalBackup
{
    class BackupServiceClient : IBackupServiceClient
    {
        private string _serviceUrlBase;

        private static readonly HttpClient client = new HttpClient();

        public BackupServiceClient(string serviceUrlBase)
        {
            _serviceUrlBase = serviceUrlBase;
        }

        public async Task<byte[]> GetFile(string fileId)
        {
            var fileContent = await client.GetStringAsync($"{_serviceUrlBase}/api/values/{fileId}");
            return Convert.FromBase64String(fileContent);
        }

        public async Task WriteFile(string fileId, byte[] fileContent)
        {
            var jsonContent = JsonConvert.SerializeObject(fileContent);
            var responseMessage = await client.PutAsync($"{_serviceUrlBase}/api/values/{fileId}", new StringContent(jsonContent,Encoding.UTF8, "application/json"));
            if (responseMessage.IsSuccessStatusCode == false)
            {
                throw new InvalidOperationException("OperationFailed");
            }
        }
    }
}
