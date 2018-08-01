using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using SituationCenter.NotifyProtocol;

namespace SituationCenter.NotifyHub.Controllers
{
    [Route("notify")]
    public class NotifyController : Controller
    {
        private readonly INotificator notificator;

        public NotifyController(INotificator notificator)
        {
            this.notificator = notificator;
        }

        [HttpPost("{topic}")]
        public async Task<IActionResult> Notify(
            string topic,
            [FromBody] JToken message)
        {
            await notificator.Notify(topic, message);
            return Ok();
        }
    }
}
