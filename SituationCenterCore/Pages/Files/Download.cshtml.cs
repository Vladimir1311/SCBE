using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SituationCenterCore.Data;
using Storage.Interfaces;

namespace SituationCenterCore.Pages.Files
{
    public class DownloadModel : FilesPage
    {
        public DownloadModel(IStorage storage, UserManager<ApplicationUser> userManager) : base(storage, userManager)
        {
        }

        public void OnGet()
        {
            File(storage.GetFileContent("some token", OwnerId, EndPath), "octet-stream");
        }
    }
}