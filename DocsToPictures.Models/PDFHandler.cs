using Spire.Pdf;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocsToPictures.Models
{
    [SupportedFormatAttribyte(".pdf")]
    public class PDFHandler : DocumentHandler
    {
        protected override void Handle()
        {
            Document neededDoc = null;
            while (true)
            {
                workQueueStopper.WaitOne();
                while (documentsStream.TryDequeue(out neededDoc))
                {
                    using (var pdfFile = new PdfDocument(Path.Combine(neededDoc.Folder, neededDoc.Name)))
                    {
                        neededDoc.PagesPaths = new string[pdfFile.Pages.Count + 1];
                        for (var i = 0; i < pdfFile.Pages.Count; i++)
                        {
                            var image = pdfFile.SaveAsImage(i);
                            string imagePath = Path.Combine(neededDoc.Folder, $"{i + 1}.png");
                            image.Save(imagePath, ImageFormat.Png);
                            image.Dispose();
                            neededDoc.PagesPaths[i + 1] = imagePath;
                            neededDoc.Progress = Percents(i, pdfFile.Pages.Count);
                        }
                    }
                }
                workQueueStopper.Reset();
            }
        }
        public override void Dispose()
        {
        }

    }
}
