namespace PCShop.Models
{
    public class Promotion
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
        public bool IsActive { get; set; } = true;
    }
}