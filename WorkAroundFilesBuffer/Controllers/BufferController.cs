using System;
using System.Collections.Generic;
using System.IO;
using IO = System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Text.RegularExpressions;
using System.Text;

namespace WorkAroundFilesBuffer.Controllers
{
    public class BufferController : Controller
    {
        [System.Web.Mvc.HttpPost]
        public string Load(HttpPostedFileBase file)
        {
            var id  = Guid.NewGuid();
            var path = Server.MapPath("~\\App_Data");
            var folder = Directory.CreateDirectory(Path.Combine(path, id.ToString()));

            var fileName = TryToGetOriginalFileName(file.FileName);
            fileName = Path.GetFileName(fileName);
            var filePath = Path.Combine(folder.FullName, fileName);
            file.SaveAs(filePath);
            return id.ToString();
        }
        public ActionResult Download(string docId)
        {
            try
            {
                var pathToFolder = Path.Combine(Server.MapPath("~\\App_Data"), docId);
                var pathToFile = Directory.GetFiles(pathToFolder)[0];
                return File(pathToFile,
                    System.Net.Mime.MediaTypeNames.Application.Octet,
                    Path.GetFileName(pathToFile));
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadGateway);
            }
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
