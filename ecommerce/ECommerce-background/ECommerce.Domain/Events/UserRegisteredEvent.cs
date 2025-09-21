using ECommerce.Domain.Entities;

namespace ECommerce.Domain.Events
{
    public class UserRegisteredEvent : BaseEvent
    {
        public Guid UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public DateTime RegisteredAt { get; set; }

        public UserRegisteredEvent(Guid userId, string email, string userName)
        {
            UserId = userId;
            Email = email;
            UserName = userName;
            RegisteredAt = DateTime.UtcNow;
            CorrelationId = userId.ToString();
            Source = "UserService";
        }
    }
}
