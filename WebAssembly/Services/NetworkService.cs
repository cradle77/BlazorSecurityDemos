using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace WebAssembly.Services
{
    public class NetworkService : IDisposable
    {
        private readonly ILogger<NetworkService> _logger;
        private readonly IJSRuntime _jsRuntime;
        private readonly DotNetObjectReference<NetworkService> _dotNetObjectReference;

        public NetworkService(ILogger<NetworkService> logger, IJSRuntime jsRuntime)
        {
            _dotNetObjectReference = DotNetObjectReference.Create(this);
            _logger = logger;
            _jsRuntime = jsRuntime;
        }

        public event EventHandler OnlineChanged;

        public ValueTask InitAsync()
        {
            return _jsRuntime.InvokeVoidAsync("blazorNetwork.init", _dotNetObjectReference);
        }

        [JSInvokable]
        public void UpdateOnlineStatus(bool status)
        {
            OnOnlineChanged(status);
        }

        public bool IsOnline { get; private set; }

        protected void OnOnlineChanged(bool status)
        {
            IsOnline = status;
            OnlineChanged?.Invoke(this, EventArgs.Empty);
            _logger.LogInformation("Status changed to {online}", status);
        }

        public void Dispose()
        {
            _dotNetObjectReference.Dispose();
        }
    }
}
