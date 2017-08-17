using CCF;
using DocsToPictures.Interfaces;
using DocsToPictures.Models;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace DocsToPictures.Controllers
{
    [System.Web.Mvc.Route("doctopic/CCF")]
    public class CCFController : Controller
    {
        private static ServiceCode service = ServiceCode.Create<IDocumentProccessor>(DocumentProcessor.Instance);

        public ActionResult Recieve()
        {
            var files = Request.Files.AllKeys.Select(N => new StreamValue() { Name = N, Value = Request.Files[N].InputStream });
            var message = Request.Form["simpleargs"];
            var res = service.Handle(message, files);

            var stringResult = res as string;
            if (stringResult != null)
                return Content(stringResult);

            var streamResult = res as Stream;
            if (streamResult != null)
                return new FileStreamResult(streamResult, "text/plain");

            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }
    }
}