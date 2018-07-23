using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using SituationCenter.Shared.Exceptions;
using SituationCenterCore.Data;

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
            var refreshTokenString = context.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "RefreshTokenId")?.Value;
            if (!Guid.TryParse(refreshTokenString, out var refreshTokenId) ||
                await dbContext.RemovedTokens.AnyAsync(rt => rt.Id == refreshTokenId))
                throw new StatusCodeException(StatusCode.IncorrectRefreshToken);
            await next();
        }
    }
}
