using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SituationCenterCore.Data.DatabaseAbstraction;

namespace SituationCenterCore.Pages.Center
{
    public class IndexModel : AuthorizedPage
    {
        public IndexModel(IRepository repositoey) : base(repositoey)
        {
        }

        public void OnGet()
        {

        }
    }
}