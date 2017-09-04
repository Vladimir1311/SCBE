using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Office.Interop.PowerPoint;
using Microsoft.Office.Core;
using System.IO;

namespace DocsToPictures.Models
{
    [SupportedFormatAttribyte(".pptx")]
    [SupportedFormatAttribyte(".ppt")]
    public class PowerPointHandler : DocumentHandler
    {
        protected override void Handle()
        {
            Application pptApplication = new Application
            {
                Visible = MsoTriState.msoTrue
            };
            while (true)
            {
                workQueueStopper.WaitOne();
                while (documentsStream.TryDequeue(out var neededPPT))
                {
                    Presentation ppt = pptApplication.Presentations.Open(Path.Combine(neededPPT.Folder, neededPPT.Name), MsoTriState.msoFalse, MsoTriState.msoFalse, MsoTriState.msoFalse);
                    neededPPT.PagesPaths = new string[ppt.Slides.Count + 1];
                    var folderName = Guid.NewGuid().ToString();
                    ppt.SaveAs(neededPPT.Folder, PpSaveAsFileType.ppSaveAsPNG);
                    for (int i = 1; i <= neededPPT.PagesCount; i++) 
                    {
                        neededPPT.PagesPaths[i] = Path.Combine(neededPPT.Folder, $"Slide{i}.PNG");
                    }
                    //foreach (Slide slide in ppt.Slides)
                    //{
                    //    string imagePath = Path.Combine(neededPPT.Folder, $"{count}.png");
                    //    slide.Export(imagePath, "png", 960, 720);
                    //    count++;
                    //    neededPPT.PagesPaths[count] = imagePath;
                    //    neededPPT.Progress = Percents(count, ppt.Slides.Count);
                    //    //object doNotSaveChanges = WDSaveOptions.wdDoNotSaveChanges;
                    //    //ppt.Close(ref doNotSaveChanges, Type.Missing, Type.Missing);
                    //}
                    ppt.Close();
                }
                workQueueStopper.Reset();
            }
        }
        private static int Percents(double done, double all)
        {
            return (int)Math.Floor((done / all) * 100);
        }
    }
}
