using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
    public class AuthorizedPage : PageModel
    {
        protected readonly IRepository repository;

        public AuthorizedPage(IRepository repository)
        {
            this.repository = repository;
        }
        public string UserId => repository.GetUserId(User).ToString();
    }
}
