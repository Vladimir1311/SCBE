using IPResolver.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IPResolver.Controllers
{
    [Route("ip/[action]/{value?}")]
    public class IPController : Controller
    {
        private readonly IConfigsManager configsManager;

        public IPController(IConfigsManager configsManager)
        {
            this.configsManager = configsManager;

        }
        public JsonResult CoreIP()
        {
            return Json(new
            {
                ip = configsManager.CoreIP
            }
            );
        }

        public string SetCoreIP(string value)
        {
            try
            {
                configsManager.CoreIP = value;
                return "OK";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
