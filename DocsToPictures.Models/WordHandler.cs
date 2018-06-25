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
        private PdfDocument pdfDoc;
        public WordHandler(CancellationToken cancellationToken) : base(cancellationToken)
        {
        }

        protected override void Prepare()
        {
            var doc = wordApp.Documents.Add(FilePath);
            var pdfFileName = Path.ChangeExtension(FilePath, "pdf");
            doc.SaveAs2(pdfFileName, WdSaveFormat.wdFormatPDF);
            doc.Close(WdSaveOptions.wdDoNotSaveChanges, Type.Missing, Type.Missing);
            pdfDoc = new PdfDocument(pdfFileName);
        }

        protected override int ReadMeta()
            => pdfDoc.Pages.Count;

        protected override void RenderPage(int number, string targetFilePath)
        {
            var image = pdfDoc.SaveAsImage(number - 1);
            image.Save(targetFilePath, ImageFormat.Png);
            image.Dispose();
        }
        public override void Dispose()
        {
            if (wordApp != null)
            {
                foreach (var document in wordApp.Documents.Cast<Microsoft.Office.Interop.Word.Document>())
                    document.Close(WdSaveOptions.wdDoNotSaveChanges);
                wordApp.Quit(WdSaveOptions.wdDoNotSaveChanges);
            }
            pdfDoc.Dispose();
        }
    }
}
