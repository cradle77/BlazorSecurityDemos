using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Des.AspNetCore.Permissions
{
    internal class PermissionPolicy : IPermissionPolicy
    {
        public string Name { get; set; }

        public IDictionary<string, string[]>[] RequirementSets { get; set; }

        public bool IsValid
        {
            get
            {
                // a policy is valid when it contains at least a valid requirement
                return this.RequirementSets.Any() &&
                    this.RequirementSets.All(
                        // a requirement is valid when it contains a list one claimType to check
                        x => x.Keys.Any()
                        // and checks the claim against at least one value
                        && x.Values.All(v => v.Any()));
            }
        }

        public AuthorizationPolicy GetPolicy()
        {
            var policyRequirement = new AssertionRequirement(context =>
            {
                return this.CheckIfMatches(context.User);
            });

            return new AuthorizationPolicy(new[] { policyRequirement }, new string[0]);
        }

        public static IEnumerable<PermissionPolicy> FromDictionary(IDictionary<string, IDictionary<string, string[]>[]> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return source
                .Select(x => new PermissionPolicy()
                {
                    Name = x.Key,
                    RequirementSets = x.Value
                });
        }

        public bool CheckIfMatches(ClaimsPrincipal principal)
        {
            // the policy is verified when at least one requirementSet is 
            // matched
            return this.RequirementSets.Any(req => req.IsMatchedBy(principal));
        }
    }
}
