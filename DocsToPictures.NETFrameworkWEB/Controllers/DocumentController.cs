using DocsToPictures.Interfaces;
using DocsToPictures.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace DocsToPictures.NETFrameworkWEB.Controllers
{
    public class DocumentController : ApiController
    {
        private static DocumentProcessor documentProcessor = new DocumentProcessor();
        public IDocument Get(Guid id)
        {
            try
            {
                return documentProcessor.CheckDocument(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Try get info about doc id {id}");
                return null;
            }
        }

        [HttpPost]
        public IDocument Post()
        {
            try
            {
                var file = HttpContext.Current.Request.Files.Count > 0 ?
                        HttpContext.Current.Request.Files[0] : null;
                return documentProcessor.AddToHandle(file.FileName, file.InputStream);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"try upload file");
                return null;
            }
        }

        [HttpGet()]
        [Route("extensions")]
        public IEnumerable<string> Get()
        {
            try
            {
                return documentProcessor.GetSupportedExtensions();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"try upload file");
                return null;
            }
        }
    }
}
