using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sReportsV2.Api.Security
{
    public class HasAnyAcceptedScopeRequirement : IAuthorizationRequirement
    {
        public string RequiredScope { get; set; }
        public HasAnyAcceptedScopeRequirement(string requiredScope)
        {
            this.RequiredScope = requiredScope;
        }
    }
}
