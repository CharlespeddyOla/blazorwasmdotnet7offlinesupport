using Microsoft.JSInterop;

namespace WebAppAcademics.Client.OfflineServices
{
    public class NetworkStatus : INetworkStatus
    {
        private readonly IJSRuntime _jsRuntime;
        public bool IsOnline { get; set; } = true;

        public delegate void OnlineStatusEventHandler(object sender,
            OnlineStatusEventArgs e);
        public event OnlineStatusEventHandler OnlineStatusChanged;

        public NetworkStatus(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;

            _ = _jsRuntime.InvokeVoidAsync("connectivity.initialize",
               DotNetObjectReference.Create(this));
        }

        [JSInvokable("ConnectivityChanged")]
        public async void OnConnectivityChanged(bool isOnline)
        {
            IsOnline = isOnline;

            if (!isOnline)
            {
                OnlineStatusChanged?.Invoke(this,
                    new OnlineStatusEventArgs { IsOnline = false });
            }
            else
            {
                await Task.CompletedTask;
                //await SyncLocalToServer();
                OnlineStatusChanged?.Invoke(this,
                    new OnlineStatusEventArgs { IsOnline = true });
            }
        }

    }
}
