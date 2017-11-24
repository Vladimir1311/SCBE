using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SituationCenterCore.Data;
using SituationCenterCore.Data.DatabaseAbstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SituationCenterCore.Pages
{
    public class AuthorizedPage : PageModel
    {
        protected readonly IRepository repository;

        public AuthorizedPage(IRepository userManager)
        {
            this.repository = userManager;
        }
        public string UserId => repository.GetUserId(User).ToString();
    }
}
