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

        private Presentation presentation;

        public PowerPointHandler(CancellationToken cancellationToken) : base(cancellationToken)
        {
        }

        protected override void Prepare()
             => presentation = pptApplication.Presentations.Open(FilePath, MsoTriState.msoFalse, MsoTriState.msoFalse, MsoTriState.msoFalse);

        protected override int ReadMeta()
            => presentation.Slides.Count;

        protected override void RenderPage(int number, string targetFilePath)
            => presentation.Slides[number].Export(targetFilePath, "PNG");

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
