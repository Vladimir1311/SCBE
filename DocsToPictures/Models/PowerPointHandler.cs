using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Core;
using Microsoft.Office.Interop.PowerPoint;
using System.IO;

namespace DocsToPictures.Models
{
    [SupportedFormatAttribyte(".pptx")]
    [SupportedFormatAttribyte(".ppt")]
    public class PowerPointHandler : DocumentHandler
    {
        protected override void Handle()
        {
            Application pptApplication = new Application();
            pptApplication.Visible = MsoTriState.msoTrue;
            Document neededPPT = null;
            while (documentsStream.TryDequeue(out neededPPT))
            {
                Presentation ppt = pptApplication.Presentations.Open(Path.Combine(neededPPT.Folder, neededPPT.Name), MsoTriState.msoFalse, MsoTriState.msoFalse, MsoTriState.msoFalse);
                var count = 1;
                neededPPT.PagesPaths = new string[ppt.Slides.Count + 1];
                foreach (Slide slide in ppt.Slides)
                {
                    string imagePath = Path.Combine(neededPPT.Folder, $"{count}.png");
                    slide.Export(imagePath, "png", 960, 720);
                    count++;
                    neededPPT.PagesPaths[count] = imagePath;
                    neededPPT.Progress = Percents(count, ppt.Slides.Count);
                    //object doNotSaveChanges = WDSaveOptions.wdDoNotSaveChanges;
                    //ppt.Close(ref doNotSaveChanges, Type.Missing, Type.Missing);
                    ppt.Close();
                }
                pptApplication.Quit();
            }
        }
        private static int Percents(double done, double all)
        {
            return (int)Math.Floor((done / all) * 100);
        }
    }
}