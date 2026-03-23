using ECommerce.Data;
using ECommerce.Models;
using ECommerce.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Services
{
    public class CartService : ICartService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _factory;

        public CartService(IDbContextFactory<ApplicationDbContext> factory)
        {
            _factory = factory;
        }

        public async Task<List<CartItem>> GetCartAsync(string userId)
        {
            await using var context = await _factory.CreateDbContextAsync();
            return await context.CartItems
                .Include(c => c.Product)
                    .ThenInclude(p => p.Category)
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.AddedAt)
                .ToListAsync();
        }

        public async Task<int> GetCartCountAsync(string userId)
        {
            await using var context = await _factory.CreateDbContextAsync();
            return await context.CartItems
                .Where(c => c.UserId == userId)
                .SumAsync(c => c.Quantity);
        }

        public async Task<decimal> GetCartTotalAsync(string userId)
        {
            await using var context = await _factory.CreateDbContextAsync();
            return await context.CartItems
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .SumAsync(c => c.Product.Price * c.Quantity);
        }

        public async Task<(bool Success, string Message)> AddToCartAsync(string userId, int productId, int quantity = 1)
        {
            try
            {
                await using var context = await _factory.CreateDbContextAsync();

                var product = await context.Products.FindAsync(productId);

                if (product == null)
                    return (false, "Product not found");
                if (product.Stock < quantity)
                    return (false, "Insufficient Stock");

                var existing = await context.CartItems.FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);

                if (existing != null)
                {
                    existing.Quantity += quantity;
                }
                else
                {
                    context.CartItems.Add(new CartItem
                    {
                        UserId = userId,
                        ProductId = productId,
                        Quantity = quantity,
                        AddedAt = DateTime.UtcNow
                    });
                }

                await context.SaveChangesAsync();
                return (true, "Added to cart successfully");
            }
            catch (Exception ex)
            {
                return (false, $"Error adding to cart: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> UpdateQuantityAsync(int cartItemId, int quantity)
        {
            try
            {
                await using var context = await _factory.CreateDbContextAsync();
                var item = await context.CartItems.FindAsync(cartItemId);
                if (item == null)
                    return (false, "Cart item not found.");

                if (quantity <= 0)
                {
                    context.CartItems.Remove(item);
                }
                else
                {
                    item.Quantity = quantity;
                }

                await context.SaveChangesAsync();
                return (true, "Cart updated.");
            }
            catch (Exception ex)
            {
                return (false, $"Error updating cart: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> RemoveFromCartAsync(int cartItemId)
        {
            try
            {
                await using var context = await _factory.CreateDbContextAsync();
                var item = await context.CartItems.FindAsync(cartItemId);
                if (item == null)
                    return (false, "Cart item not found.");

                context.CartItems.Remove(item);
                await context.SaveChangesAsync();
                return (true, "Removed from cart.");
            }
            catch (Exception ex)
            {
                return (false, $"Error removing from cart: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> ClearCartAsync(string userId)
        {
            try
            {
                await using var context = await _factory.CreateDbContextAsync();
                var items = await context.CartItems
                    .Where(c => c.UserId == userId)
                    .ToListAsync();

                context.CartItems.RemoveRange(items);
                await context.SaveChangesAsync();
                return (true, "Cart cleared.");
            }
            catch (Exception ex)
            {
                return (false, $"Error clearing cart: {ex.Message}");
            }
        }
    }
}