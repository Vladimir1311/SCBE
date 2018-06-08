using MonitoringApp.Shared.General;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

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
    }
}
