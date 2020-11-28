using Blazored.LocalStorage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using WebAssembly.Services;

namespace WebAssembly
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services
                .AddHttpClient("weatherapi", client =>
                {
                    client.BaseAddress = new Uri("https://localhost:5002");
                })
                .AddHttpMessageHandler(sp => 
                {
                    var handler = sp.GetRequiredService<AuthorizationMessageHandler>()
                        .ConfigureHandler(new[] { "https://localhost:5002" }, 
                        scopes: new[] { "weatherapi" });

                    return handler;
                });

            builder.Services.AddSingleton(sp =>
            {
                var factory = sp.GetService<IHttpClientFactory>();

                return factory.CreateClient("weatherapi");
            });

            builder.Services.AddOidcAuthentication(options =>
            {
                builder.Configuration.Bind("Security", options.ProviderOptions);
                options.UserOptions.RoleClaim = "role";
                options.ProviderOptions.PostLogoutRedirectUri = builder.HostEnvironment.BaseAddress;
            }).AddAccountClaimsPrincipalFactory<ArrayClaimsPrincipalFactory<RemoteUserAccount>>();

            builder.Services.RemoveAll<IAuthorizationPolicyProvider>();

            builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
            builder.Services.AddScoped<NetworkService>();

            builder.Services.AddBlazoredLocalStorage();
            builder.Services.AddScoped<UserService>();

            await builder.Build().RunAsync();
        }
    }
}
