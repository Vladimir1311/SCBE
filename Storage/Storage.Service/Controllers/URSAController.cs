using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;
using URSA;
using URSA.Collector.Interfaces;

namespace Storage.Service.Controllers
{
    [Produces("application/json")]
    [Route("api/URSA")]
    public class URSAController : Controller
    {
        private readonly ICollector _collector;
        private readonly ILogger<URSAController> _logger;

        public URSAController(ICollector collector, ILogger<URSAController> logger)
        {
            _collector = collector;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var collect = _collector.CollectAssembly(GetType().Assembly);
                var buff = collect.ToByteArray();

                await Response.Body.WriteAsync(buff, 0, buff.Length);

                return new EmptyResult();
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return Content(e.ToString());
            }
        }
    }
}