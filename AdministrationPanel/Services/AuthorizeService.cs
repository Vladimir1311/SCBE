using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AdministrationPanel.Services
{
    static class AuthorizeService
    {
        private static string token;

        public static string Token => token ?? Authorize();

        public static string Authorize(string email = "maksalmak@gmail.com", string password = "CaPOnidolov2_")
        {
            using (HttpClient client = new HttpClient())
            {
                string body = JsonConvert.SerializeObject(new
                {
                    Email = email,
                    Password = password
                });
                HttpContent content = new StringContent(body, Encoding.UTF8, "application/json");
                var response = client.PostAsync("http://localhost/api/v1/Account/Authorize", content)
                    .Result
                    .Content
                    .ReadAsStringAsync()
                    .Result;
                token = JObject.Parse(response)["accessToken"].ToString();
                return token;
            }
        }

    }
}
