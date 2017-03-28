using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace UDPServerTester.Controllers
{
    public class UDPController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}