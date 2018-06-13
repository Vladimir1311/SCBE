using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Word;
using System.IO;
using Spire.Pdf;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;

namespace DocsToPictures.Models
{
    [SupportedFormatAttribyte(".docx")]
    [SupportedFormatAttribyte(".doc")]
    public class WordHandler : DocumentHandler
    {
        private Application wordApp = new Application();

        public WordHandler(CancellationToken cancellationToken) : base(cancellationToken)
        {
        }

        protected override void Handle(Document document)
        {
            var wordFileName = Path.Combine(document.Folder, document.Name);
            var doc = wordApp.Documents.Add(wordFileName);
            var pdfFileName = Path.ChangeExtension(wordFileName, "pdf");
            doc.SaveAs2(pdfFileName, WdSaveFormat.wdFormatPDF);
            doc.Close(WdSaveOptions.wdDoNotSaveChanges, Type.Missing, Type.Missing);

            using (var pdfFile = new PdfDocument(pdfFileName))
            {
                document.SetPagesCount(pdfFile.Pages.Count);
                for (var i = 0; i < pdfFile.Pages.Count; i++)
                {
                    var image = pdfFile.SaveAsImage(i);
                    string imagePath = Path.Combine(document.Folder, $"{i + 1}.png");
                    image.Save(imagePath, ImageFormat.Png);
                    image.Dispose();
                    document[i + 1] = imagePath;
                }

            }
        }

        public override void Dispose()
        {
            if (wordApp != null)
            {

                foreach (var document in wordApp.Documents.Cast<Microsoft.Office.Interop.Word.Document>())
                    document.Close(WdSaveOptions.wdDoNotSaveChanges);
                wordApp.Quit(WdSaveOptions.wdDoNotSaveChanges);
            }
        }
    }
}
