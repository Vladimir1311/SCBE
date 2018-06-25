using Spire.Pdf;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DocsToPictures.Models
{
    [SupportedFormatAttribyte(".pdf")]
    public class PDFHandler : DocumentHandler
    {
        public PDFHandler(CancellationToken cancellationToken) : base(cancellationToken)
        {
        }
        private PdfDocument pdfDoc;

        protected override void Prepare()
        {
            pdfDoc = new PdfDocument(FilePath);
        }

        protected override int ReadMeta()
            => pdfDoc.Pages.Count;

        protected override void RenderPage(int number, string targetFileName)
        {
            var image = pdfDoc.SaveAsImage(number - 1);
            image.Save(targetFileName, ImageFormat.Png);
            image.Dispose();
        }
        public override void Dispose()
        {
            pdfDoc?.Dispose();
        }
    }
}
