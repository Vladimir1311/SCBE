using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace SituationCenterCore
{
    [Produces("application/json")]
    [Route("api/IRResolver")]
    public class IRResolverController : Controller
    {
        public IActionResult Index()
        {
            return Json(new Dictionary<string, string>
            {
                {"core", "localhost:5000"},
                {"storage", "localhost:5050"}
            });
        }
    }
}