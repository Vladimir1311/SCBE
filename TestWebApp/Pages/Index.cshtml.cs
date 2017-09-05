using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using System.IO;
using CCF;
using DocsToPictures.Interfaces;
using System.Threading;

namespace TestWebApp.Pages
{
    public class IndexModel : PageModel
    {
        public void OnGet()
        {

        }

        public void OnPost(List<IFormFile> files)
        {
            var service = CCFServicesManager.GetService<IDocumentProcessor>();
            var fileName = @"D:\Users\maksa\Desktop\DocsToPictures\doc.docx";
            var memstr = new MemoryStream();
            files[0].CopyTo(memstr);
            memstr.Position = 0;
            var document = service.AddToHandle("file.docx", memstr);
            memstr.Position = 0;
            using (var file = System.IO.File.Create(fileName))
                memstr.CopyTo(file);
            while (document.ReadyPagesCount != document.PagesCount)
            {
                for (int i = 20; i >= 0; i--)
                {
                    Console.WriteLine(i);
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                }
            }
            var pages = document.GetAvailablePages();
            Console.WriteLine($"getted {pages.Count} pages");
            foreach (var page in pages)
            {
                Console.WriteLine($"page num : {page}");
                using (var pic = System.IO.File.Create($@"D:\Users\maksa\Desktop\DocsToPictures\{page}.png"))
                    document.GetPicture(page).CopyTo(pic);
            }
        }
    }
}
