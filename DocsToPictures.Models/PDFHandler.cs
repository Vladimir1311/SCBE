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

        protected override void Handle(Document document)
        {
            using (var pdfFile = new PdfDocument(Path.Combine(document.Folder, document.Name)))
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
        }

    }
}
