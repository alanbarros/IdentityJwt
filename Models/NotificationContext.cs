using System.Collections.Generic;
using System.Linq;

namespace IdentityJwt.Models
{
    public class NotificationContext : INotifications
    {
        public NotificationContext()
        {
            Notifications = new List<Notification>();
        }

        public List<Notification> Notifications { get; set; }

        public bool HasNotifications => Notifications.Any();

        public string SerializedNotifications => System.Text.Json.JsonSerializer.Serialize(Notifications);

        public void AddNotification(string key, string message)
            => Notifications.Add(new Notification(key, message));
        public void AddNotifications(IEnumerable<(string key, string value)> errors)
        {
            foreach (var error in errors)
            {
                AddNotification(error.key, error.value);
            }
        }
    }

}