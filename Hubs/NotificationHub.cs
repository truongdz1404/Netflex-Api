using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Netflex.Hubs
{
    [Authorize]
    public class NotificationHub : Hub<INotificationClient>
    {
        private readonly ConnectionManager _connectionManager;
        public NotificationHub(ConnectionManager connectionManager)
        {
            _connectionManager = connectionManager;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new Exception("User ID is not available");

            var connectionId = Context.ConnectionId;
            _connectionManager.AddConnection(userId, connectionId,
                (u) => Clients.Others.UserConnected(u)
            );
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            string userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new Exception("User is not authenticated");

            string connectionId = Context.ConnectionId;
            _connectionManager.RemoveConnection(userId, connectionId,
                (u) => Clients.Others.UserDisconnected(u));
            await base.OnDisconnectedAsync(exception);
        }
    }
    public interface INotificationClient
    {
        Task ReceiveNotification(string message);
        Task UserConnected(string userName);
        Task UserDisconnected(string userName);
    }

}