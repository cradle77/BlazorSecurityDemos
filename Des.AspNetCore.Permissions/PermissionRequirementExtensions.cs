using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Des.AspNetCore.Permissions
{
    internal static class PermissionRequirementExtensions
    {
        public static bool IsMatchedBy(this IDictionary<string, string[]> requirements, ClaimsPrincipal principal)
        {
            if (requirements == null)
                throw new ArgumentNullException(nameof(requirements));
            if (principal == null)
                throw new ArgumentNullException(nameof(principal));

            return requirements.All(
                entry =>
                {
                    // role and name are two special aliases
                    var keys = entry.Key.GetFullyQualifiedNameArray();

                    var claimsFound = principal.FindAll(c => keys.Contains(c.Type))
                        .Select(x => x.Value);

                    return claimsFound.Intersect(entry.Value).Any();
                });
        }

        private const string NAME_CLAIM = "name";
        private const string ROLE_CLAIM = "role";

        private static string[] GetFullyQualifiedNameArray(this string key)
        {
            if (key == ROLE_CLAIM)
            {
                return new[] { key, ClaimsIdentity.DefaultRoleClaimType };
            }

            if (key == NAME_CLAIM)
            {
                return new[] { key, ClaimsIdentity.DefaultNameClaimType };
            }

            return new[] { key };
        }
    }
}
