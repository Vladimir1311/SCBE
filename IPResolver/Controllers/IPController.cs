using Common.ResponseObjects;
using Common.ResponseObjects.IPRows;
using Common.Services;
using IPResolver.Models;
using IPResolver.Models.RequestModels;
using IPResolver.Services;
using Microsoft.AspNetCore.Mvc;
using System;
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

        public JsonResult CCFList()
        {
            return Json(new { CCFs = servicesDb.CCFServises.ToArray() });
        }

        public JsonResult CoreIP()
        {
            return Json(new
            {
                ip = servicesDb.ServiseRows.First(R => R.ServiceType == "Core").IP.ToString()
            });
        }

        public JsonResult EndPoints()
        {
            return Json(new
            {
                core = servicesDb.ServiseRows.FirstOrDefault(R => R.ServiceType == "Core")?.IP.ToString(),
                storage = servicesDb.ServiseRows.FirstOrDefault(R => R.ServiceType == "Storage")?.IP.ToString()
            }
            );
        }

        public ContentResult GetCCFEndPoint(string interfaceName)
        {
            var endPoint = servicesDb.CCFServises.FirstOrDefault(S => S.InterfaceName == interfaceName)?.CCFEndPoint;

            return Content(endPoint ?? "");
        }

        public ContentResult SetCCFEndPoint(string interfaceName, string url)
        {
            try
            {
                var createdRow = servicesDb.CCFServises.FirstOrDefault(R => R.InterfaceName == interfaceName);
                if (createdRow != null)
                {
                    createdRow.CCFEndPoint = BuildEndPoint(url);
                }
                else
                {
                    createdRow = new CCFService
                    {
                        InterfaceName = interfaceName,
                        CCFEndPoint = BuildEndPoint(url)
                    };
                    servicesDb.CCFServises.Add(createdRow);
                }
                servicesDb.SaveChanges();
                return Content("OK");
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        public string SetCoreIP(string value)
        {
            try
            {
                var createdRow = servicesDb.ServiseRows.FirstOrDefault(R => R.ServiceType == "Core");
                if (createdRow != null)
                {
                    createdRow.IP = IPAddress.Parse(value);
                }
                else
                {
                    createdRow = new ServiceRow(IPAddress.Parse(value), "Core");
                    servicesDb.ServiseRows.Add(createdRow);
                }
                servicesDb.SaveChanges();
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
            if (createdRow != null)
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

        private string BuildEndPoint(string url)
        {
            var ip = HttpContext.Connection.RemoteIpAddress.MapToIPv4();
            return $"http://{ip}/{url}";
        }
    }
}