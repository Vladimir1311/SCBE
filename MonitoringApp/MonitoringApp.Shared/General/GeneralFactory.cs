using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace MonitoringApp.Shared.General
{
    class GeneralFactory
    {
        public static HttpClient CreateHttpClient()
        {
#if __WASM__
            var handler = new Uno.UI.Wasm.WasmHttpHandler();
            var httpClient = new HttpClient(handler);
#else
            var httpClient = new HttpClient();
#endif
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/json"));
            return httpClient;
        }
    }
}
