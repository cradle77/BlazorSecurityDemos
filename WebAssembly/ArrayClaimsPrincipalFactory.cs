using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace WebAssembly
{
    public class ArrayClaimsPrincipalFactory<TAccount> : AccountClaimsPrincipalFactory<TAccount> where TAccount : RemoteUserAccount
    {
        private IHttpClientFactory _clientFactory;

        public ArrayClaimsPrincipalFactory(IAccessTokenProviderAccessor accessor, IHttpClientFactory clientFactory)
        : base(accessor)
        {
            _clientFactory = clientFactory;
        }


        // when a user belongs to multiple roles, IS4 returns a single claim with a serialised array of values
        // this class improves the original factory by deserializing the claims in the correct way
        public async override ValueTask<ClaimsPrincipal> CreateUserAsync(TAccount account, RemoteAuthenticationUserOptions options)
        {
            var user = await base.CreateUserAsync(account, options);

            var claimsIdentity = (ClaimsIdentity)user.Identity;

            if (account != null)
            {
                foreach (var kvp in account.AdditionalProperties)
                {
                    var name = kvp.Key;
                    var value = kvp.Value;
                    if (value != null &&
                        (value is JsonElement element && element.ValueKind == JsonValueKind.Array))
                    {
                        claimsIdentity.RemoveClaim(claimsIdentity.FindFirst(kvp.Key));

                        var claims = element.EnumerateArray()
                            .Select(x => new Claim(kvp.Key, x.ToString()));

                        claimsIdentity.AddClaims(claims);
                    }
                }

                var client = _clientFactory.CreateClient("weatherapi");

                var permissions = (await client.GetFromJsonAsync<IEnumerable<string>>("https://localhost:5002/api/mypermissions"))
                    .Select(x => new Claim("permission", x))
                    .ToList();

                Console.WriteLine($"I've retrieved {permissions.Count} permissions");

                claimsIdentity.AddClaims(permissions);
            }

            return user;
        }
    }
}