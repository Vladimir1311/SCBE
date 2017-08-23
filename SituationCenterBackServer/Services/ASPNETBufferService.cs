using SituationCenterBackServer.Models.StorageModels;
using System;
using System.Net.Http;
using IO = System.IO;

namespace SituationCenterBackServer.Services
{
    public class ASPNETBufferService : IBuffer
    {
        private const string linkToServer = "http://172.22.104.101:62631/";
        private IStorageManager storageManager;

        public ASPNETBufferService(IStorageManager storageManager)
        {
            this.storageManager = storageManager;
        }

        public string ServLink => linkToServer + "buffer/download?docId=";

        public string GetLinkFor(File file)
        {
            using (var client = new HttpClient()
            {
                BaseAddress = new Uri(linkToServer)
            })
            {
                byte[] data;
                var docStream = storageManager.GetFileStream(file.Path);
                using (var br = new IO.BinaryReader(docStream))
                    data = br.ReadBytes((int)docStream.Length);

                ByteArrayContent bytes = new ByteArrayContent(data);
                MultipartFormDataContent multiContent = new MultipartFormDataContent
                {
                    { bytes, "file", IO.Path.GetFileName(file.Name)}
                };

                var result = client.PostAsync("buffer/load", multiContent)
                    .Result
                    .Content
                    .ReadAsStringAsync()
                    .Result;
                return result;
            }
        }
    }
}