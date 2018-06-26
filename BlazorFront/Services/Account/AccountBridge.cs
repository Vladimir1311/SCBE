using Microsoft.AspNetCore.Blazor;
using SituationCenter.Shared.Requests.Account;
using SituationCenter.Shared.ResponseObjects;
using SituationCenter.Shared.ResponseObjects.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace BlazorFront.Services.Account
{
    public class AccountBridge
    {
        private readonly HttpClient client;

        public AccountBridge(HttpClient client)
        {
            this.client = client;
            client.BaseAddress = new Uri("http://localhost:5000");
        }

        public Task<ResponseBase> Register(RegisterRequest registerRequest)
            => client.PostJsonAsync<ResponseBase>("/api/v1/Account/registration", registerRequest);

        public Task<AuthorizeResponse> Login(LoginRequest loginRequest)
            => client.PostJsonAsync<AuthorizeResponse>("/api/v1/Account/Authorize", loginRequest);
    }
}
