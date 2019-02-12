using System.Collections.Generic;
using System.Linq;

namespace ChatCore.ChatApp.Services
{
    public class ConnectionMap
    {
        private readonly static Dictionary<string, HashSet<string>> _connections = new Dictionary<string, HashSet<string>>();

        static ConnectionMap()
        {}
        public int Count
        {
            get
            {
                return _connections.Count;
            }
        }

        public void Add(string UserId, string connectionId)
        {
            lock (_connections)
            {
                HashSet<string> connections;
                if (!_connections.TryGetValue(UserId, out connections))
                {
                    connections = new HashSet<string>();
                    _connections.Add(UserId, connections);
                }

                lock (connections)
                {
                    connections.Add(connectionId);
                }
            }
        }

        public IEnumerable<string> GetConnections(string UserId)
        {
            HashSet<string> connections;
            if (_connections.TryGetValue(UserId, out connections))
            {
                return connections;
            }

            return Enumerable.Empty<string>();
        }

        public void Remove(string UserId, string connectionId)
        {
            lock (_connections)
            {
                HashSet<string> connections;
                if (!_connections.TryGetValue(UserId, out connections))
                {
                    return;
                }

                lock (connections)
                {
                    connections.Remove(connectionId);

                    if (connections.Count == 0)
                    {
                        _connections.Remove(UserId);
                    }
                }
            }
        }
    }
}