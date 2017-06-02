using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;

namespace DocsToPictures.Controllers
{
    public class WordToPictureController : Controller
    {
        public string TryIt()
        {
            var docPath = Server.MapPath("~/App_Data/word.docx");
            var app = new Microsoft.Office.Interop.Word.Application();


            //app.Visible = true;

            var doc = app.Documents.Open(docPath);

            doc.ShowGrammaticalErrors = false;
            doc.ShowRevisions = false;
            doc.ShowSpellingErrors = false;


            //Opens the word document and fetch each page and converts to image
            foreach (Microsoft.Office.Interop.Word.Window window in doc.Windows)
            {
                foreach (Microsoft.Office.Interop.Word.Pane pane in window.Panes)
                {
                    for (var i = 1; i <= pane.Pages.Count; i++)
                    {
                        var page = pane.Pages[i];
                        var bits = page.EnhMetaFileBits;
                        var target = Path.Combine(@"C:\\Users\maksa\Desktop\Words\lol");

                        try
                        {
                            using (var ms = new MemoryStream((byte[])(bits)))
                            {
                                var image = System.Drawing.Image.FromStream(ms);
                                var pngTarget = Path.ChangeExtension(target, "png");
                                image.Save(pngTarget, ImageFormat.Png);
                            }
                        }
                        catch (System.Exception ex)
                        { }
                    }
                }
            }
            doc.Close(Type.Missing, Type.Missing, Type.Missing);
            app.Quit(Type.Missing, Type.Missing, Type.Missing);
            return "Ji)";
        }
    }
}
