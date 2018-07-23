using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SituationCenterCore.Extensions;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SituationCenterCore.Controllers
{
    public class BaseParamsController : Controller
    {
        protected Guid UserId => User.Id();
        protected Guid RefreshTokenId => User.RefreshTokenId();
        protected string UserAgent =>
            Request.Headers.TryGetValue("User-Agent", out var uAgent) ? uAgent.ToString() : "No User Agent";

    }
}
