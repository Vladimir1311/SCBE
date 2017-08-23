using Common.ResponseObjects;
using DocsToPictures.Models;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace DocsToPictures.Controllers
{
    
    public class DocumentToPicturesController : Controller
    {
        private static DocumentProcessor docsProcessor = DocumentProcessor.Instance;

        public ActionResult Index()
        {
            return View();
        }

        public ResponseBase GetInfo(Guid docId)
        {
            //try
            //{
            //    var doc = docsProcessor.GetDocument(docId);
            //    return ResponseBase.GoodResponse(new
            //    {
            //        Progress = doc.Progress,
            //        AvailablePages = doc.PagesPaths
            //            .Where(P => P != null)
            //            .Select((P, N) => N + 1)});
            //}
            //catch (Exception ex)
            //{
            //    return ResponseBase.BadResponse(ex.Message);
            //}
            return ResponseBase.BadResponse("Sorry, old func");
        }

        public ActionResult Download(Guid docId, int pageNum)
        {
            //try
            //{
            //    var doc = docsProcessor.GetDocument(docId);
            //    if (pageNum <= 0 || doc.PagesPaths.Length < pageNum || doc.PagesPaths[pageNum] == null)
            //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //    return File(doc.PagesPaths[pageNum],
            //        System.Net.Mime.MediaTypeNames.Application.Octet,
            //        $"Page {pageNum}.png");
            //}
            //catch
            //{
            //    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            //}
            return Content("Sorry, old func");
        }

        [System.Web.Mvc.HttpPost]
        public ResponseBase Load(HttpPostedFileBase file)
        {
            //// Verify that the user selected a file
            //if (file != null && file.ContentLength > 0)
            //{
            //    // extract only the filename
            //    var prepare = file.FileName.TryToGetOriginalFileName();
            //    var fileName = Path.GetFileName(prepare);
            //    // store the file inside ~/App_Data/uploads folder
            //    Guid folderId = Guid.NewGuid();
            //    var folderPath = Server.MapPath("~\\App_Data\\uploads");
            //    var folderForDoc = Directory.CreateDirectory(Path.Combine(folderPath, folderId.ToString())).FullName;
            //    file.SaveAs(Path.Combine(folderForDoc, fileName));
            //    Document doc = new Document
            //    {
            //        Id = folderId,
            //        Folder = folderForDoc,
            //        Name = fileName
            //    };
            //    try
            //    {
            //        docsProcessor.AddToHandle(doc);
            //    }
            //    catch(Exception ex)
            //    {
            //        return ResponseBase.BadResponse(ex.Message);
            //    }
            //    return ResponseBase.GoodResponse(new
            //    {
            //        PackId = folderId
            //    });
            //}
            return ResponseBase.BadResponse("Not valid file");
        }

        private static readonly Regex _regexEncodedFileName = new Regex(@"^=\?utf-8\?B\?([a-zA-Z0-9/+]+={0,2})\?=$");
        private static string TryToGetOriginalFileName(string fileNameInput)
        {
            Match match = _regexEncodedFileName.Match(fileNameInput);
            if (match.Success && match.Groups.Count > 1)
            {
                string base64 = match.Groups[1].Value;
                try
                {
                    byte[] data = Convert.FromBase64String(base64);
                    return Encoding.UTF8.GetString(data);
                }
                catch (Exception)
                {
                    //ignored
                    return fileNameInput;
                }
            }
            return fileNameInput;
        }

    }
}
