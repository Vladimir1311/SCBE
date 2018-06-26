using Cloudcrate.AspNetCore.Blazor.Browser.Storage;
using SituationCenter.Shared.Models.People;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorFront.Services.Account
{
    public class UserState
    {
        private readonly LocalStorage localStorage;

        public UserState(LocalStorage localStorage)
        {
            this.localStorage = localStorage;
        }

        public PersonPresent User { get; set; }
    }
}
