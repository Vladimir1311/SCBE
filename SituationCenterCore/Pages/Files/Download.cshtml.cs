using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SituationCenterCore.Data;
using Storage.Interfaces;
using System.IO;

namespace SituationCenterCore.Pages.Files
{
    public class DownloadModel : FilesPage
    {
        public DownloadModel(IStorage storage, UserManager<ApplicationUser> userManager) : base(storage, userManager)
        {
        }

        public FileStreamResult OnGet()
        {
            return File(storage.GetFileContent("some token", OwnerId, EndPath), "application/octet-stream", Path.GetFileNameWithoutExtension(EndPath));
        }
    }
}