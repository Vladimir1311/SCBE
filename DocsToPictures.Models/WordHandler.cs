using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Word;
using System.IO;
using Spire.Pdf;
using System.Drawing;
using System.Drawing.Imaging;

namespace DocsToPictures.Models
{
    [SupportedFormatAttribyte(".docx")]
    [SupportedFormatAttribyte(".doc")]
    public class WordHandler : DocumentHandler
    {
        private Application wordApp = new Application();
        protected override void Handle()
        {
            Document neededDoc = null;
            while (true)
            {
                workQueueStopper.WaitOne();
                while (documentsStream.TryDequeue(out neededDoc))
                {
                    var wordFileName = Path.Combine(neededDoc.Folder, neededDoc.Name);
                    var doc = wordApp.Documents.Add(wordFileName);
                    var pdfFileName = Path.ChangeExtension(wordFileName, "pdf");
                    doc.SaveAs2(pdfFileName, WdSaveFormat.wdFormatPDF);
                    doc.Close(WdSaveOptions.wdDoNotSaveChanges, Type.Missing, Type.Missing);

                    using (var pdfFile = new PdfDocument(pdfFileName))
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
        private static int Percents(double done, double all)
        {
            return (int)Math.Floor((done / all) * 100);
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
