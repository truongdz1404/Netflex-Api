using System.Threading.Channels;
using Netflex.Hubs;
using Microsoft.AspNetCore.SignalR;
namespace Netflex.Services;

public record Message(IEnumerable<string> SendTo, string Content);

public class NotificationQueueService : BackgroundService
{
    private readonly Channel<Message> _channel;
    private readonly IHubContext<NotificationHub, INotificationClient> _hubContext;
    private readonly ConnectionManager _connectionManager;

    public NotificationQueueService(
        IHubContext<NotificationHub, INotificationClient> hubContext,
        ConnectionManager connectionManager)
    {
        _channel = Channel.CreateUnbounded<Message>();
        _hubContext = hubContext;
        _connectionManager = connectionManager;
    }

    public ValueTask PushAsync(Message message)
        => _channel.Writer.WriteAsync(message);


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var notification = await _channel.Reader.ReadAsync(stoppingToken);
            await HandleNotificationsAsync(notification);
        }
    }

    private async Task HandleNotificationsAsync(Message message)
    {
        var connectionIds = message.SendTo.SelectMany(_connectionManager.GetConnections).Distinct();
        if (connectionIds == null || !connectionIds.Any()) return;
        await _hubContext.Clients.Clients(connectionIds)
                .ReceiveNotification(message.Content);
    }
}
