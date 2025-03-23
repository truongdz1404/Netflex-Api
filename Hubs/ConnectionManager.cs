using System.Collections.Concurrent;

namespace FStudyForum.API.Hubs;

public class ConnectionManager
{
    private static readonly ConcurrentDictionary<string, UserConnection> _users =
            new(StringComparer.InvariantCultureIgnoreCase);
    public IEnumerable<string> OnlineUsers => _users.Keys;
    public void AddConnection(string userName, string connectionId,
        Action<string>? userConnected = null)
    {
        var user = _users.GetOrAdd(userName, _ => new UserConnection
        {
            UserName = userName,
            ConnectionIds = new HashSet<string>()
        });
        lock (user.ConnectionIds)
        {
            user.ConnectionIds.Add(connectionId);
            if (user.ConnectionIds.Count == 1)
                userConnected?.Invoke(userName);
        }
    }

    public void RemoveConnection(string userName, string connectionId,
        Action<string>? userDisconneted = null)
    {
        _users.TryGetValue(userName, out var user);
        if (user != null)
        {
            lock (user.ConnectionIds)
            {
                user.ConnectionIds.RemoveWhere(cid => cid.Equals(connectionId));
                if (user.ConnectionIds.Count == 0)
                {
                    _users.TryRemove(userName, out var removedUser);
                    userDisconneted?.Invoke(userName);
                }
            }
        }
    }

    public HashSet<string> GetConnections(string userName)
    {
        HashSet<string> conn = [];
        _users.TryGetValue(userName, out var user);
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