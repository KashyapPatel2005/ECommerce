using ECommerce.Data;
using System.ComponentModel.DataAnnotations;

namespace ECommerce.Models
{
    public class Review
    {
        public int Id { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        [MaxLength(1000)]
        public string? Comment { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsVerified { get; set; } = false;

        // Foreign Keys
        public string UserId { get; set; } = string.Empty;
        public int ProductId { get; set; }

        // Navigation
        public ApplicationUser User { get; set; } = default!;
        public Product Product { get; set; } = default!;
    }
}