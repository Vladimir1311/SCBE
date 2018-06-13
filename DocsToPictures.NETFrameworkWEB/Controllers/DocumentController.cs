using DocsToPictures.Interfaces;
using DocsToPictures.Models;
using DocsToPictures.NETFrameworkWEB.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace DocsToPictures.NETFrameworkWEB.Controllers
{
    [RoutePrefix("api/document")]
    public class DocumentController : ApiController
    {
        private readonly Logger<DocumentController> logger = new Logger<DocumentController>();
        private readonly DocumentsQueue docsQueue;
        public DocumentController()
        {
            docsQueue = DocumentsQueue.Instance;
        }
        [HttpGet]
        [Route("{id}")]
        public IDocument Get(Guid id)
        {
            try
            {
                logger.Info("Get values");
                return docsQueue.CurrentDocuments.FirstOrDefault(d => d.Id == id);
            }
            catch (Exception ex)
            {
                logger.Warning($"Try get info about doc id {id}", ex);
                return null;
            }
        }

        [HttpGet]
        [Route("{id}/{pageNum}")]
        public HttpResponseMessage Get(Guid id, int pageNum)
        {
            try
            {
                var targetDoc = docsQueue.CurrentDocuments.FirstOrDefault(d => d.Id == id);
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
        {
            logger.Info("Get all values");
            try
            {
                return docsQueue.CurrentDocuments;
            }
            catch (Exception ex)
            {
                logger.Warning("when getCurrentDocs", ex);
                return null;
            }

        }

        [HttpPost]
        public async Task<IDocument> Post()
        {
            try
            {
                logger.Info($"Files count: {HttpContext.Current.Request.Files.Count}");
                var file = HttpContext.Current.Request.Files.Count > 0 ?
                        HttpContext.Current.Request.Files[0] : null;
                logger.Info($"Files name: {file.FileName}");
                logger.Info($"Files length: {file.ContentLength}");
                return await docsQueue.AddAsync(file.FileName, file.InputStream);
            }
            catch (Exception ex)
            {
                logger.Warning($"exception: {ex.Message}", ex);
                return null;
            }
        }
    }
}
