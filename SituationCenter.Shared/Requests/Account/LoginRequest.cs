using System;
using System.Collections.Generic;
using System.Text;

namespace SituationCenter.Shared.Requests.Account
{
    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
