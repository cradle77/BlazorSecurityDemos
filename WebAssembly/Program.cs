using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebAssembly
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

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
                //options.UserOptions.RoleClaim = "role";
            });

            await builder.Build().RunAsync();
        }
    }
}
