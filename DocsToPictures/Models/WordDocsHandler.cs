using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Reflection;
using Microsoft.Office.Interop.Word;
using System.Drawing;
using System.Drawing.Imaging;
using Spire.Pdf;

using org.pdfclown.documents;
using org.pdfclown.tools;

namespace DocsToPictures.Models
{
    [SupportedFormatAttribyte(".docx")]
    [SupportedFormatAttribyte(".doc")]
    public class WordDocsHandler : DocumentHandler
    {

        protected override void Handle()
        {
            var wordApp = new Application();
            wordApp.Visible = true;
            Document neededDoc = null;
            while (documentsStream.TryDequeue(out neededDoc))
            {
                var wordFileName = Path.Combine(neededDoc.Folder, neededDoc.Name);
                var doc = wordApp.Documents.Add(wordFileName);
                var pdfFileName = Path.ChangeExtension(wordFileName, "pdf");
                doc.SaveAs2(pdfFileName, WdSaveFormat.wdFormatPDF);
                doc.Close(WdSaveOptions.wdDoNotSaveChanges, Type.Missing, Type.Missing);

                using (var file = new org.pdfclown.files.File(pdfFileName))
                {
                    var pdfDoc = file.Document;
                    Renderer renderer = new Renderer();
                    neededDoc.PagesPaths = new string[pdfDoc.Pages.Count + 1];
                    for (var i = 0; i < pdfDoc.Pages.Count; i++)
                    {
                        var image = renderer.Render(pdfDoc.Pages[i], pdfDoc.Pages[i].Size);
                        string imagePath = Path.Combine(neededDoc.Folder, $"{i}.png");
                        image.Save(imagePath, ImageFormat.Png);
                        image.Dispose();
                        neededDoc.PagesPaths[i] = imagePath;
                        neededDoc.Progress = Percents(i, pdfDoc.Pages.Count);
                    }

                }







                //PdfDocument pdfDoc = new PdfDocument();
                //pdfDoc.LoadFromFile(pdfFileName);

                
            }
            wordApp.Quit(Type.Missing, Type.Missing, Type.Missing);
        }
        private static int Percents(double done, double all)
        {
            return (int)Math.Floor((done / all) * 100);
        }
        private static Image Transparent2Color(Image bmp1, Color target)
        {
            Bitmap bmp2 = new Bitmap(bmp1.Width, bmp1.Height);
            System.Drawing.Rectangle rect = new System.Drawing.Rectangle(System.Drawing.Point.Empty, bmp1.Size);
            using (Graphics G = Graphics.FromImage(bmp2))
            {
                G.Clear(target);
                G.DrawImageUnscaledAndClipped(bmp1, rect);
            }
            bmp1.Dispose();
            return bmp2;
        }
        public static System.Drawing.Bitmap ReplaceTransparency(System.Drawing.Image bitmap, System.Drawing.Color background)
        {
            /* Important: you have to set the PixelFormat to remove the alpha channel.
             * Otherwise you'll still have a transparent image - just without transparent areas */
            var result = new System.Drawing.Bitmap(bitmap.Size.Width, bitmap.Size.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            var g = System.Drawing.Graphics.FromImage(result);

            g.Clear(background);
            g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
            g.DrawImage(bitmap, 0, 0);
            return result;
        }

    }
}