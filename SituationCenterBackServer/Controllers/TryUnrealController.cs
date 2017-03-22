using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace SituationCenterBackServer.Controllers
{
    public class TryUnrealController : Controller
    {
        public IActionResult Index()
        {
            return Content("It's works!");
        }
    }
}