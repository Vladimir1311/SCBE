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
using SituationCenterBackServer.Interfaces;
using SituationCenterCore.Data.DatabaseAbstraction;

namespace SituationCenterCore.Pages.Files
{
    public class DownloadModel : FilesPage
    {
        private readonly IAccessValidator accessValidator;

        public DownloadModel(IStorage storage, IRepository repository, IAccessValidator accessValidator) : base(storage, repository)
        {
            this.accessValidator = accessValidator;
        }

        public IActionResult OnGet()
        {
            if(accessValidator.CanAccessToFolder(UserId, EndPath))
            {
                return RedirectToPage("./AccessDenied");
            }
            return File(storage.GetFileContent("some token", OwnerId, EndPath), "application/octet-stream", Path.GetFileName(EndPath));
        }
    }
}