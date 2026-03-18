using ECommerce.Data;

namespace ECommerce.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public int Quantity { get; set; } = 1;
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;

        // Foreign Keys
        public string UserId { get; set; } = string.Empty;
        public int ProductId { get; set; }

        // Navigation
        public ApplicationUser User { get; set; } = default!;
        public Product Product { get; set; } = default!;
    }
}