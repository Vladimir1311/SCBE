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
    [RoutePrefix("api/document")]
    public class DocumentController : ApiController
    {
        private static DocumentProcessor documentProcessor = new DocumentProcessor();
        [HttpGet]
        [Route("{id}")]
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

        [HttpGet]
        [Route("{id}/{pageNum}")]
        public HttpResponseMessage Get(Guid id, int pageNum)
        {
            try
            {
                var targetDoc = documentProcessor.CheckDocument(id);
                if (targetDoc == null)
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                var pic = targetDoc.GetPicture(pageNum);
                if (pic == null)
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                HttpResponseMessage httpResponseMessage = Request.CreateResponse(HttpStatusCode.OK);
                httpResponseMessage.Content = new StreamContent(pic);
                httpResponseMessage.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
                httpResponseMessage.Content.Headers.ContentDisposition.FileName = $"{pageNum}.PNG";
                httpResponseMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                return httpResponseMessage;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Try get info about doc id {id}");
                return null;
            }
        }

        [HttpGet]
        [Route("all")]
        public IEnumerable<IDocument> GetAll()
            => documentProcessor.GetCurrentDocs();

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
