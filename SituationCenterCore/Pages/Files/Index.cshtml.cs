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
using SituationCenterCore.Data.DatabaseAbstraction;

namespace SituationCenterCore.Pages.Files
{
    [Authorize]
    public class IndexModel : FilesPage
    {
        public IDirectoryDesc CurrentDirectory { get; private set; }

        public IndexModel(IStorage storage, IRepository repository) : base(storage, repository)
        {
        }

        [BindProperty]
        public string NewFolderName { get; set; }
        public IActionResult OnGet()
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

        public IActionResult OnPost(List<IFormFile> files)
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
            return RedirectToPage(new { folderPath = EndPath, owner = Owner });
        }
        private void UploadFile(List<IFormFile> files)
        {

        }

        private void FillFields()
        {
            CurrentDirectory = storage.GetDirectoryInfo("sample token", OwnerId, EndPath);
        }
    }
}
