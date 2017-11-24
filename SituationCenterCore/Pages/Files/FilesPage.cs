using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SituationCenterCore.Data;
using SituationCenterCore.Data.DatabaseAbstraction;
using Storage.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SituationCenterCore.Pages.Files
{
    public class FilesPage : AuthorizedPage
    {
        protected readonly IStorage storage;
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
                endPath = HttpUtility.UrlDecode(endPath);
                if (endPath.Length > 0 && endPath[0] == '/')
                    endPath = endPath.Substring(1);

            }
        }

        [BindProperty(SupportsGet = true)]
        public string Owner { get; set; }


        private string ownerId;
        public string OwnerId => ownerId ??
            (Owner == "self" ? ownerId = UserId : ownerId = Owner);

        public FilesPage(IStorage storage, IRepository repository) : base(repository)
        {
            this.storage = storage;
        }
    }
}
