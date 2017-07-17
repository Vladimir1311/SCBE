using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SituationCenterBackServer.Models.DocsModels;

namespace SituationCenterBackServer.Controllers
{
    public class DocsController : Controller
    {
        public IActionResult Index()
        {
            return View(IndexViewModel.Create());
        }
    }
}