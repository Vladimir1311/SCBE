using Common.ResponseObjects;
using DocsToPictures.Models;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web;
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
            try
            {
                var doc = docsProcessor.GetDocument(docId);
                return ResponseBase.GoodResponse(new
                {
                    Progress = doc.Progress,
                    AvailablePages = doc.PagesPaths
                        .Where(P => P != null)
                        .Select((P, N) => N + 1)

                });
            }
            catch (Exception ex)
            {
                return ResponseBase.BadResponse(ex.Message);
            }
        }

        [HttpPost]
        public ResponseBase Load(HttpPostedFileBase file)
        {
            // Verify that the user selected a file
            if (file != null && file.ContentLength > 0)
            {
                // extract only the filename
                var fileName = Path.GetFileName(file.FileName);
                // store the file inside ~/App_Data/uploads folder
                Guid folderId = Guid.NewGuid();
                var folderPath = Server.MapPath("~\\App_Data\\uploads");
                var folderForDoc = Directory.CreateDirectory(Path.Combine(folderPath, folderId.ToString())).FullName;
                file.SaveAs(Path.Combine(folderForDoc, fileName));
                Document doc = new Document
                {
                    Id = folderId,
                    Folder = folderForDoc,
                    Name = fileName
                };
                try
                {
                    docsProcessor.AddToHandle(doc);
                }
                catch(Exception ex)
                {
                    return ResponseBase.BadResponse(ex.Message);
                }
                return ResponseBase.GoodResponse(new
                {
                    PackId = folderId
                });
            }
            return ResponseBase.BadResponse("Not valid file");
        }
        

    }
}
