using Microsoft.AspNetCore.Identity;

namespace PCShop.Models
{
    public enum OrderType { Purchase, Service }

    public class Order
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public IdentityUser User { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Pending";
        public OrderType Type { get; set; }
        public List<OrderItem> Items { get; set; } = new();
    }
}