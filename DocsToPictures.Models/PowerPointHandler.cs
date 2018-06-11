using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Office.Interop.PowerPoint;
using Microsoft.Office.Core;
using System.IO;
using System.Threading;
using DocsToPictures.Interfaces;

namespace DocsToPictures.Models
{
    [SupportedFormatAttribyte(".pptx")]
    [SupportedFormatAttribyte(".ppt")]
    public class PowerPointHandler : DocumentHandler
    {
        private Application pptApplication = new Application
        {
            Visible = MsoTriState.msoTrue
        };

        public PowerPointHandler(CancellationToken cancellationToken) : base(cancellationToken)
        {
        }

        protected override void Handle(Document document)
        {
            Presentation ppt = pptApplication.Presentations.Open(Path.Combine(document.Folder, document.Name), MsoTriState.msoFalse, MsoTriState.msoFalse, MsoTriState.msoFalse);
            document.SetPagesCount(ppt.Slides.Count);
            var folderName = Guid.NewGuid().ToString();
            ppt.SaveAs(document.Folder, PpSaveAsFileType.ppSaveAsPNG);
            for (int i = 1; i <= document.PagesCount; i++)
            {
                document[i] = Path.Combine(document.Folder, $"Slide{i}.PNG");
            }
            //foreach (Slide slide in ppt.Slides)
            //{
            //    string imagePath = Path.Combine(document.Folder, $"{count}.png");
            //    slide.Export(imagePath, "png", 960, 720);
            //    count++;
            //    document.PagesPaths[count] = imagePath;
            //    document.Progress = Percents(count, ppt.Slides.Count);
            //    //object doNotSaveChanges = WDSaveOptions.wdDoNotSaveChanges;
            //    //ppt.Close(ref doNotSaveChanges, Type.Missing, Type.Missing);
            //}
            ppt.Close();

        }


        public override void Dispose()
        {
            if (pptApplication != null)
            {
                foreach (var presentaion in pptApplication.Presentations.Cast<Presentation>())
                    presentaion.Close();
                pptApplication.Quit();
            }
        }
    }
}
