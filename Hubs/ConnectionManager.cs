using System.Collections.Concurrent;

namespace Netflex.Hubs;

public class ConnectionManager
{
    private static readonly ConcurrentDictionary<string, UserConnection> _users =
            new(StringComparer.InvariantCultureIgnoreCase);
    public IEnumerable<string> OnlineUsers => _users.Keys;
    public void AddConnection(string userId, string connectionId,
        Action<string>? userConnected = null)
    {
        var user = _users.GetOrAdd(userId, _ => new UserConnection
        {
            UserName = userId,
            ConnectionIds = new HashSet<string>()
        });
        lock (user.ConnectionIds)
        {
            user.ConnectionIds.Add(connectionId);
            if (user.ConnectionIds.Count == 1)
                userConnected?.Invoke(userId);
        }
    }

    public void RemoveConnection(string userId, string connectionId,
        Action<string>? userDisconneted = null)
    {
        _users.TryGetValue(userId, out var user);
        if (user != null)
        {
            lock (user.ConnectionIds)
            {
                user.ConnectionIds.RemoveWhere(cid => cid.Equals(connectionId));
                if (user.ConnectionIds.Count == 0)
                {
                    _users.TryRemove(userId, out var removedUser);
                    userDisconneted?.Invoke(userId);
                }
            }
        }
    }

    public HashSet<string> GetConnections(string userId)
    {
        HashSet<string> conn = [];
        _users.TryGetValue(userId, out var user);
        if (user != null)
        {
            lock (user.ConnectionIds)
            {
                conn = user.ConnectionIds;
            }
        }
        return conn;
    }

    public class UserConnection
    {
        public required string UserName { get; set; }
        public HashSet<string> ConnectionIds { get; set; } = [];
    }

}