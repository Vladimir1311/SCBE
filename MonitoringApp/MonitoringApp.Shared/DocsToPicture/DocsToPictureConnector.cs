using MonitoringApp.Shared.General;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringApp.Shared.DocsToPicture
{
    class DocsToPictureConnector
    {
        private readonly HttpClient httpClient;
        public string ApiUrl
        {
            get => httpClient.BaseAddress.ToString();
            set => httpClient.BaseAddress = new Uri(value);
        }
        public DocsToPictureConnector()
        {
            httpClient = GeneralFactory.CreateHttpClient();
        }

        public async Task<string> GetSome()
            => await httpClient.GetStringAsync("https://jsonplaceholder.typicode.com/posts");
    }
}
