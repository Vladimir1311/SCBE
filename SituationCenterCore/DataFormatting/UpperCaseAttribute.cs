using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace SituationCenterCore.DataFormatting
{
    public class UpperCaseAttribute : ModelBinderAttribute, IModelBinder
    {
        public UpperCaseAttribute() : base(typeof(UpperCaseAttribute))
        {
                
        }
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.FieldName);
            bindingContext.Result = ModelBindingResult.Success(valueProviderResult.FirstValue?.ToUpper());
            return Task.CompletedTask;
        }
    }
}
