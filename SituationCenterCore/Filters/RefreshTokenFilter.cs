using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using SituationCenter.Shared.Exceptions;
using SituationCenterCore.Data;
using SituationCenterCore.Extensions;

namespace SituationCenterCore.Filters
{
    public class RefreshTokenFilter : IAsyncActionFilter
    {
        private readonly ApplicationDbContext dbContext;

        public RefreshTokenFilter(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.ActionDescriptor.FilterDescriptors.Any(fd => fd.Filter is AllowAnonymousFilter))
            {
                await next();
                return;
            }

            try
            {
                var refreshTokenId = context.HttpContext.User.RefreshTokenId();
                if (await dbContext.RemovedTokens.AnyAsync(rt => rt.Id == refreshTokenId))
                {
                    throw new Exception();
                }

            }
            catch
            {
                throw new StatusCodeException(StatusCode.IncorrectRefreshToken);
            }

            await next();
        }
    }
}
