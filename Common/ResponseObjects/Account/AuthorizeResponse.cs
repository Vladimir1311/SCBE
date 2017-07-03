using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.ResponseObjects.Account
{
    public class AuthorizeResponse : ResponseBase
    {
        [JsonProperty("accessToken")]
        public string AccessToken { get; set; }

        private AuthorizeResponse(string token)
        { AccessToken = token; }


        public static AuthorizeResponse Create(string token)
            => new AuthorizeResponse(token);
    }
}
