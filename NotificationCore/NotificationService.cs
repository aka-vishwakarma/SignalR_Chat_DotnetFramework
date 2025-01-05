using Microsoft.AspNet.SignalR;
using System;

namespace NotificationCore
{
    public class NotificationService
    {
        private readonly IHubContext _hubContext;

        public NotificationService()
        {
            _hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
        }

        public void NotifyAllClients(string user, string message)
        {
            try
            {
                _hubContext.Clients.All.receiveNotification(user, message);
                Console.WriteLine($"Notification sent: {user} - {message}");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error sending notification: {ex.Message}");
                throw;
            }
        }


        public void NotifyUser(Guid userId, string sender, string message)
        {
            var connectionId = NotificationHub.GetConnectionId(userId);

            if (!string.IsNullOrEmpty(connectionId))
            {
                _hubContext.Clients.Client(connectionId).receiveNotification(sender, message);
                Console.WriteLine($"Notification sent to User {userId}: {message}");
            }
            else
            {
                Console.WriteLine($"User {userId} is not connected.");
            }
        }
    }
}
