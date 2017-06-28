using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using SituationCenterBackServer.Models.AccountViewModels;

namespace SituationCenterBackServer.Controllers.API.V1
{
    [Produces("application/json")]
    [Route("api/Account")]
    [Authorize]
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> logger;

        public AccountController(ILogger<AccountController> logger)
        {
            this.logger = logger;
        }

        [AllowAnonymous]
        public IActionResult Authorize([FromBody]LoginViewModel model)
        {
            throw new NotImplementedException();
        }
    }
}