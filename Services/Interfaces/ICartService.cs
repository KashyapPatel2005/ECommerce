using ECommerce.Models;

namespace ECommerce.Services.Interfaces
{
    public interface ICartService
    {
        Task<List<CartItem>> GetCartAsync(string userId);
        Task<int> GetCartCountAsync(string userId);
        Task<(bool Success, string Message)> AddToCartAsync(
            string userId, int productId, int quantity = 1);
        Task<(bool Success, string Message)> UpdateQuantityAsync(
            int cartItemId, int quantity);
        Task<(bool Success, string Message)> RemoveFromCartAsync(int cartItemId);
        Task<(bool Success, string Message)> ClearCartAsync(string userId);
        Task<decimal> GetCartTotalAsync(string userId);
    }
}