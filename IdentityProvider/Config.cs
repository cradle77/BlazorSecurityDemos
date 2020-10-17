﻿// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityModel;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace IdentityProvider
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> Ids =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                // we need to include the role claim in the profile
                //new ProfileWithRoleIdentityResource(),
                new IdentityResources.Profile(),
                new IdentityResources.Email()
            };


        public static IEnumerable<ApiResource> Apis =>
            new ApiResource[]
            {
                new ApiResource("weatherapi", "Weather API", new[] 
                { 
                    JwtClaimTypes.Subject, 
                    JwtClaimTypes.Name, 
                    JwtClaimTypes.Role 
                })
            };


        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                new Client
                {
                    ClientId = "blazor",
                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,
                    RequireClientSecret = false,
                    AllowedCorsOrigins = { "https://localhost:5001" },
                    AllowedScopes = { "openid", "profile", "email", "weatherapi" },
                    RedirectUris = { "https://localhost:5001/authentication/login-callback" },
                    PostLogoutRedirectUris = { "https://localhost:5001/" },
                    Enabled = true
                },
            };
    }
}