using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Storage.Interfaces;
using Microsoft.AspNetCore.Authorization;
using SituationCenterCore.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace SituationCenterCore.Pages.Files
{
    [Authorize]
    public class IndexModel : FilesPage
    {
        public IDirectoryDesc CurrentDirectory { get; private set; }

        public IndexModel(IStorage storage, UserManager<ApplicationUser> userManager) : base(storage, userManager)
        {
        }
        
        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                FillFields();
                return Page();
            }
            catch
            {
                return LocalRedirect("/storage");
            }
        }

        public async Task<IActionResult> OnPostAsync(List<IFormFile> files, string folderPath = "self")
        {
            FillFields();
            if (files.Count == 0)
                return Page();
            var file = files[0];
            var success = storage.CreateFile(
                "sample token",
                OwnerId,
                Path.Combine(EndPath, Path.GetFileName(file.FileName)),
                file.OpenReadStream()
                );
            return LocalRedirect("/Files?folderPath=" + folderPath);
        }


        private void FillFields()
        {
            CurrentDirectory = storage.GetDirectoryInfo("sample token", OwnerId, EndPath);
        }
    }
}
