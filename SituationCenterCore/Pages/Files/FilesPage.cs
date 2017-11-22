using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SituationCenterCore.Data;
using Storage.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SituationCenterCore.Pages.Files
{
    public class FilesPage : PageModel
    {
        protected readonly IStorage storage;
        protected readonly UserManager<ApplicationUser> userManager;
        private string endPath;
        [BindProperty(SupportsGet = true)]
        public string EndPath
        {
            get
            {
                return endPath;
            }
            set
            {
                endPath = value ?? "";
            }
        }

        [BindProperty(SupportsGet = true)]
        public string Owner { get; set; }


        private string ownerId;
        public string OwnerId => ownerId ??
            (Owner == "self" ? ownerId = userManager.GetUserId(User) : ownerId = Owner);

        public FilesPage(IStorage storage, UserManager<ApplicationUser> userManager)
        {
            this.storage = storage;
            this.userManager = userManager;
        }


    }
}
