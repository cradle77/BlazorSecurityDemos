using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace WeatherApi.Hubs
{
    [Authorize]
    public class NotificationsHub : Hub
    {
    }
}
