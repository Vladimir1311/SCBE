using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CCF;
using System.IO;
using System.Net.Http;
using DocsToPictures.Interfaces;
using System.Text;
using Microsoft.Net.Http.Headers;
using System.Threading;

namespace UDPServerTester.Controllers
{
    [Produces("application/json")]
    [Route("CCF/[action]")]
    public class CCFController : Controller
    {
        private static IDocumentProccessor docsProccessot;
        private static ServiceCode service = ServiceCode.Create(new Proccessor());

        public CCFController()
        {
            docsProccessot
                = RemoteWorker.Create<IDocumentProccessor>("http://localhost:62961");
        }

        public IActionResult Send()
        {
            var doc = docsProccessot.AddToHandle("myFile.doc", System.IO.File.OpenRead(@"C:\Users\maksa\Desktop\Belyaeva_Olya_k_pr\Belyaeva_Olya_k_pr.doc"));
            while (true)
            {
                Thread.Sleep(1000);
                var picStream = doc.GetPicture(1);
                if (picStream != null)
                {
                    using (var str = System.IO.File.Create(@"C:\Users\maksa\Desktop\New folder (2)\image.png"))
                        picStream.CopyTo(str);
                    return Content("success getted pic!");
                }
            }
        }

        public IActionResult Recieve()
        {

            var res = service.Handle(Request.Form["simpleargs"], Request.Form.Files.Select(F => new StreamValue { Name = F.Name, Value = F.OpenReadStream()}));
            if (res == null)
                return NoContent();
            switch (res)
            {
                case string stringResult:
                    return Content(stringResult);

                case Stream streamResult:
                    return new FileStreamResult(streamResult, "text/plain");

                default:
                    return NotFound();
            }
        }
    }

    internal class Proccessor : IDocumentProccessor
    {
        private Dictionary<Guid, IDocument> docs = new Dictionary<Guid, IDocument>();

        public IDocument AddToHandle(string fileName, Stream fileStream)
        {
            
            var guid = Guid.NewGuid();
            IDocument doc = new Document(guid);
            docs.Add(guid, doc);
            return doc;
        }

        public IDocument CheckDocument(Guid id)
        {
            if (docs.TryGetValue(id, out var doc))
                return doc;
            else
                return null;
        }
    }

    internal class Document : IDocument
    {

        

        public IEnumerable<int> AvailablePages => new int[] { 1, 2, 3, 4 };

        public Document(Guid id)
        {
            Id = id;
        }
        public Guid Id { get; set; }

        public string Name => "Name";

        public Stream GetPicture(int pageNum)
        {
            return null;
        }
    }
}