using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SituationCenter.Shared.Exceptions;
using System.Reflection;
using System.ComponentModel;

namespace SituationCenterCore.Pages.Docs
{
    public class IndexModel : PageModel
    {
        public List<StatusCodePresent> StatusCodes { get; private set; }
        public void OnGet()
        {
            StatusCodes= new List<StatusCodePresent>();

            foreach (StatusCode code in Enum.GetValues(typeof(StatusCode)))
            {
                StatusCodes.Add(new StatusCodePresent()
                {
                    Code = (int)code,
                    CSharpName = code.ToString(),
                    Description = typeof(StatusCode)
                            .GetMember(code.ToString())[0]
                            .GetCustomAttribute<DescriptionAttribute>().Description
                });
            }
        }

        public class StatusCodePresent
        {
            public int Code { get; set; }
            public string Description { get; set; }
            public string CSharpName { get; set; }
        }
    }
}
