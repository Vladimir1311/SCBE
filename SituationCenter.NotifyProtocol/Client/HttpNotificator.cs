using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace SituationCenter.NotifyProtocol.Client
{
    public class HttpNotificator : INotificator
    {
        private readonly HttpClient httpClient;
        private readonly ILogger<HttpNotificator> logger;
        public const string HttpClientName = "HttpNotificatorHttpClient";

        public HttpNotificator(IHttpClientFactory httpClientFactory, ILogger<HttpNotificator> logger)
        {
            httpClient = httpClientFactory.CreateClient(HttpClientName);
            this.logger = logger;
        }
        public async Task Notify<T>(string topic, T data) where T: class 
        {
            if (string.IsNullOrEmpty(topic))
                throw new ArgumentNullException(nameof(topic));
            data = data ?? throw new ArgumentNullException(nameof(data));


            var jsonContent = JsonConvert.SerializeObject(data);
            var stringContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            try
            {
                await httpClient.PostAsync($"notify/{topic}", stringContent);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, $"can't send notify message to {httpClient.BaseAddress}");
            }
        }
    }
}
