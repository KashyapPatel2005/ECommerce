namespace ECommerce.Models
{
    public class ProductImage
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsMain { get; set; } = false;
        public int SortOrder { get; set; } = 0;

        // Foreign Key
        public int ProductId { get; set; }

        // Navigation
        public Product Product { get; set; } = default!;
    }
}