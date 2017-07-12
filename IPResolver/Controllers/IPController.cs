using Common.ResponseObjects;
using Common.ResponseObjects.IPRows;
using Common.Services;
using IPResolver.Models;
using IPResolver.Models.RequestModels;
using IPResolver.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace IPResolver.Controllers
{
    [Route("ip/[action]/{value?}")]
    public class IPController : Controller
    {
        private readonly IConfigsManager configsManager;
        private readonly ServicesContext servicesDb;

        public IPController(IConfigsManager configsManager, ServicesContext servicesDB)
        {
            this.configsManager = configsManager;
            this.servicesDb = servicesDB;

        }

        public ResponseBase Get()
        {
            var rows = servicesDb.ServiseRows.ToArray();
            return IPRowsListResponse.Create(rows);
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

        [HttpPost]
        public async Task<ResponseBase> Register([FromBody]RegisterRequest request)
        {
            if (!string.Equals(request?.Token, GlobalTokens.RegisterServiseToken))
                return ResponseBase.BadResponse("token is incorrect");
            var ip = HttpContext.Connection.RemoteIpAddress;
            var createdRow = servicesDb.ServiseRows.FirstOrDefault(R => R.ServiceType == request.ServiceType);
            if(createdRow != null)
            {
                createdRow.IP = ip;
            }
            else
            {
                createdRow = new ServiceRow(ip, request.ServiceType);
                servicesDb.ServiseRows.Add(createdRow);
            }
            await servicesDb.SaveChangesAsync();
            return ResponseBase.GoodResponse();
        }


        private IPAddress ClientIP()
        {
            var ip = HttpContext.Connection.RemoteIpAddress;
            

            return ip;
        }
    }
}
