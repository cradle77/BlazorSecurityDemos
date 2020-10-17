using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Des.AspNetCore.Permissions
{
    internal class PermissionPolicyProvider : IAuthorizationPolicyProvider, IPermissionPolicyProvider
    {
        private Dictionary<string, IPermissionPolicy> _permissions;

        public PermissionPolicyProvider(IEnumerable<IPermissionPolicy> permissions)
        {
            if (permissions == null)
            {
                throw new ArgumentNullException(nameof(permissions));
            }

            var invalidPermission = permissions.FirstOrDefault(x => !x.IsValid);
            if (invalidPermission != null)
            {
                throw new ArgumentException($"Permission {invalidPermission.Name} is invalid");
            }

            _permissions = permissions.ToDictionary(x => x.Name);
        }

        public Task<IEnumerable<string>> GetPermissionsAsync(ClaimsPrincipal principal)
        {
            if (principal == null)
            {
                throw new ArgumentNullException(nameof(principal));
            }

            var result = _permissions.Values
                .Where(x => x.CheckIfMatches(principal))
                .Select(x => x.Name);

            return Task.FromResult(result);
        }

        public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
        {
            return Task.FromResult(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());
        }

        public Task<AuthorizationPolicy> GetFallbackPolicyAsync()
        {
            return Task.FromResult<AuthorizationPolicy>(null);
        }

        public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            return Task.FromResult(_permissions[policyName].GetPolicy());
        }
    }
}
