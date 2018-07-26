using System;
using SituationCenterCore.Services.Interfaces.RealTime;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace SituationCenterCore.Controllers.API.V1
{
    [Route("api/v1/testnotify")]
    public class TestNotifyController : BaseParamsController
    {
        private readonly INotificator notificator;
        private readonly IDistributedCache cache;

        public TestNotifyController(
            INotificator notificator,
            IDistributedCache cache)
        {
            this.notificator = notificator;
            this.cache = cache;
        }

        [HttpGet]
        public async Task<IActionResult> Get(string topic) 
        {
            await notificator.Notify(topic, (object)new { val = "Azaz" });
            return Content(await cache.GetStringAsync("lolkey"));
        }

        [HttpPost]
        public async Task<IActionResult> Post(string content)
        {
            await cache.SetStringAsync("lolkey", content);
            return Ok();
        }
    }
}
