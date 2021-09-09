using System.Collections.Generic;

namespace IdentityJwt.Models
{
    public interface INotifications
    {
        List<Notification> Notifications { get; set; }

        string SerializedNotifications { get; }

        bool HasNotifications { get; }

        void AddNotification(string key, string message);
    }

}