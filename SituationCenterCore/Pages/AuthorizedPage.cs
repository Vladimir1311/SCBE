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
        private ApplicationUser user;

        public AuthorizedPage(IRepository repository)
        {
            this.repository = repository;
        }
        public string UserId => repository.GetUserId(User).ToString();
        public ApplicationUser SignedUser => user ?? (user = repository.FindUser(User).Result);
    }
}
