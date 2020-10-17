using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Des.AspNetCore.Permissions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddPermissionPolicies(this IServiceCollection services, string configSection = "permissions")
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (string.IsNullOrWhiteSpace(configSection))
                throw new ArgumentNullException(nameof(configSection));

            services.AddSingleton<IAuthorizationPolicyProvider>(sp =>
            {
                var config = sp.GetRequiredService<IConfiguration>();

                var permissions = new Dictionary<string, IDictionary<string, string[]>[]>();

                config.GetSection(configSection).Bind(permissions);

                return new PermissionPolicyProvider(PermissionPolicy.FromDictionary(permissions));
            });

            services.AddSingleton<IPermissionPolicyProvider>(sp =>
            {
                return (IPermissionPolicyProvider)sp.GetRequiredService<IAuthorizationPolicyProvider>();
            });
        }
    }
}
