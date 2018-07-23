﻿using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SituationCenterCore.Models.TokenAuthModels
{
    public class MockAuthOptions
    {
        public const string ISSUER = "http://localhost:5000";
        public const string AUDIENCE = "http://localhost:5000/";
        const string KEY = "mysupersecret_secretkey!123";
        public const int LIFETIME = 100;
        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }
    }
}
