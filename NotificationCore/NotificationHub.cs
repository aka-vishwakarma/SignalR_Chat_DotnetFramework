using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using System;
using System.Collections.Concurrent;
using System.Linq;
using Microsoft.AspNet.SignalR.Hubs;

namespace NotificationCore
{
    [HubName("notificationHub")]
    public class NotificationHub : Hub
    {
        private static readonly ConcurrentDictionary<Guid, string> UserConnections = new ConcurrentDictionary<Guid, string>();

        public void RegisterUser(Guid userId)
        {
            // Add or update the user ID to connection ID mapping
            UserConnections[userId] = Context.ConnectionId;
            Console.WriteLine($"User {userId} connected with ConnectionId {Context.ConnectionId}");
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            // Remove the user from the dictionary when they disconnect
            Guid userId = UserConnections.FirstOrDefault(kvp => kvp.Value == Context.ConnectionId).Key;
            if (Guid.Empty != userId)
            {
                UserConnections.TryRemove(userId, out _);
                Console.WriteLine($"User {userId} disconnected.");
            }

            return base.OnDisconnected(stopCalled);
        }

        public static string GetConnectionId(Guid userId)
        {
            // Get the connection ID for a specific user ID
            UserConnections.TryGetValue(userId, out var connectionId);
            return connectionId;
        }



        // On client connection
        public override Task OnConnected()
        {
            Console.WriteLine($"Client connected: {Context.ConnectionId}");
            return base.OnConnected();
        }

        // On client reconnection
        public override Task OnReconnected()
        {
            Console.WriteLine($"Client reconnected: {Context.ConnectionId}");
            return base.OnReconnected();
        }

        // Method invoked from the client to send a notification
        public void SendNotification(string user, string message)
        {
            // Send notification to all connected clients
            Clients.All.receiveNotification(user, message);
        }
    }
}
