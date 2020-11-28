using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;

namespace WebAssembly
{
    public class NotificationsManager : IAsyncDisposable
    {
        private HubConnection _hubConnection;
        private AuthenticationStateProvider _authentication;
        private IAccessTokenProvider _tokenProvider;

        public event Func<string, Task> DataFetchedAsync;

        public NotificationsManager(AuthenticationStateProvider authentication, IAccessTokenProvider tokenProvider)
        {
            _authentication = authentication;
            _tokenProvider = tokenProvider;
            _authentication.AuthenticationStateChanged += RefreshConnection;
        }

        private void RefreshConnection(Task<AuthenticationState> task)
        {
            this.InitializeAsync();
        }

        public async Task InitializeAsync()
        {
            var state = await _authentication.GetAuthenticationStateAsync();

            _hubConnection = new HubConnectionBuilder()
                .WithUrl("https://localhost:5002/hubs/notifications", options => 
                {
                    options.AccessTokenProvider = async () => 
                    {
                        var token = await _tokenProvider.RequestAccessToken();
                        token.TryGetToken(out var tokenresult);

                        Console.WriteLine($"token was {tokenresult.Value}");

                        return tokenresult.Value;
                    };
                })
                .Build();

            _hubConnection.On<string>("dataFetched", async (username) =>
            {
                Console.WriteLine($"dataFetched received: {username}");

                await this.DataFetchedAsync?.Invoke(username);
            });

            if (!state.User.Identity.IsAuthenticated)
            {
                await _hubConnection.StopAsync();
            }
            else
            {
                await _hubConnection.StartAsync();
            }
        }

        public async ValueTask DisposeAsync()
        {
            await _hubConnection.DisposeAsync();
            _authentication.AuthenticationStateChanged -= RefreshConnection;
        }
    }
}
