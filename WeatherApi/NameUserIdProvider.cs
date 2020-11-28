using Microsoft.AspNetCore.SignalR;

namespace WeatherApi
{
    internal class NameUserIdProvider : IUserIdProvider
    {
        public string GetUserId(HubConnectionContext connection)
        {
            return connection.User.FindFirst("name")?.Value;
        }
    }
}