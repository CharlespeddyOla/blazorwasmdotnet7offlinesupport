namespace WebAppAcademics.Client.OfflineServices
{
    public interface INetworkStatus
    {
        event NetworkStatus.OnlineStatusEventHandler OnlineStatusChanged;
    }
}