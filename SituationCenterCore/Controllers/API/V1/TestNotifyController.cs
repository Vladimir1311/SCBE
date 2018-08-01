﻿using System;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using SituationCenter.NotifyProtocol;

namespace SituationCenterCore.Controllers.API.V1
{
    [Route("api/v1/testnotify")]
    public class TestNotifyController : BaseParamsController
    {
        private readonly INotificator notificator;

        public TestNotifyController(INotificator notificator)
        {
            this.notificator = notificator;
        }

        [HttpGet]
        public async Task<IActionResult> Get(string topic) 
        {
            await notificator.Notify(topic, new { val = "Azaz" });
            return Ok();
        }
    }
}
