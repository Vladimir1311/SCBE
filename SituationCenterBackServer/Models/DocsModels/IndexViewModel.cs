using Common.ResponseObjects;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace SituationCenterBackServer.Models.DocsModels
{
    public class IndexViewModel
    {
        public List<StatusCodeViewModel> StatusCodes { get; set; }

        public static IndexViewModel Create()
        {
            List<StatusCodeViewModel> codes = new List<StatusCodeViewModel>();
            foreach (StatusCode code in Enum.GetValues(typeof(StatusCode)))
            {
                codes.Add(new StatusCodeViewModel()
                {
                    Code = (int)code,
                    CSharpName = code.ToString(),
                    Description = typeof(StatusCode)
                            .GetMember(code.ToString())[0]
                            .GetCustomAttribute<StatusCodeDescription>().Description
                });
            }
            return new IndexViewModel() { StatusCodes = codes };
        }
    }
}