using System.Threading.Channels;
using Netflex.Hubs;
using Microsoft.AspNetCore.SignalR;
namespace Netflex.Services;

public record Notify(IEnumerable<string> SendTo, string Message);

public class NotificationQueueService : BackgroundService
{
    private readonly Channel<Notify> _channel;
    private readonly IHubContext<NotificationHub, INotificationClient> _hubContext;
    private readonly ConnectionManager _connectionManager;

    public NotificationQueueService(
        IHubContext<NotificationHub, INotificationClient> hubContext,
        ConnectionManager connectionManager)
    {
        _channel = Channel.CreateUnbounded<Notify>();
        _hubContext = hubContext;
        _connectionManager = connectionManager;
    }

    public ValueTask PushAsync(Notify notification)
        => _channel.Writer.WriteAsync(notification);


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var notification = await _channel.Reader.ReadAsync(stoppingToken);
            await HandleNotificationsAsync(notification);
        }
    }

    private async Task HandleNotificationsAsync(Notify notification)
    {
        var connectionIds = notification.SendTo.SelectMany(_connectionManager.GetConnections).Distinct();
        if (connectionIds == null || !connectionIds.Any()) return;
        await _hubContext.Clients.Clients(connectionIds)
                .ReceiveNotification(notification.Message);
    }
}
