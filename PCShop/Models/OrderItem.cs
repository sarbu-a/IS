namespace PCShop.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;
        public int? ProductId { get; set; }
        public Product? Product { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public bool IsPromotionDiscount { get; set; }
    }
}