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

namespace SituationCenterCore.Pages.Storage
{
    [Authorize]
    public class TestModel : PageModel
    {
        private readonly UserManager<ApplicationUser> userManager;



        public TestModel(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }
        public async Task<IActionResult> OnGetAsync(string folderPath = "self")
        {
            return Page();
        }


        public List<string> lols { get; set; } = new List<string>() { "Empty List"};
        public async Task<IActionResult> OnPostAsync(List<IFormFile> file, string folderPath = "self")
        {
            lols = file.Select(F => F.FileName).ToList();
            return Page();
        }

    }
}
