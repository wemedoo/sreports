using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sReportsV2.Api.Security
{
    public class HasAnyAcceptedScopeHandler : AuthorizationHandler<HasAnyAcceptedScopeRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HasAnyAcceptedScopeRequirement requirement)
        {
            System.Security.Claims.Claim roleClaim = context.User.FindFirst("http://schemas.microsoft.com/ws/2008/06/identity/claims/role");
            if (roleClaim != null && roleClaim.Value.Split(' ').Contains(requirement.RequiredScope))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
