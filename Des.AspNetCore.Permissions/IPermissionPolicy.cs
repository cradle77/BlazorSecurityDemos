using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Des.AspNetCore.Permissions
{
    internal interface IPermissionPolicy
    {
        string Name { get; set; }

        AuthorizationPolicy GetPolicy();

        bool CheckIfMatches(ClaimsPrincipal principal);

        bool IsValid { get; }
    }
}