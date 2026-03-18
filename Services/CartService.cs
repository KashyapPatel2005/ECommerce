using ECommerce.Data;
using ECommerce.Models;
using ECommerce.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Services
{
    public class CartService : ICartService
    {
        private readonly ApplicationDbContext _context;

        public CartService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<CartItem>> GetCartAsync(string userId)
        {
            return await _context.CartItems
                .Include(c => c.Product)
                    .ThenInclude(p => p.Category)
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.AddedAt)
                .ToListAsync();
        }

        public async Task<int> GetCartCountAsync(string userId)
        {
            return await _context.CartItems
                .Where(c => c.UserId == userId)
                .SumAsync(c => c.Quantity);
        }

        public async Task<decimal> GetCartTotalAsync(string userId)
        {
            return await _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .SumAsync(c => c.Product.Price * c.Quantity);
        }


        public async Task<(bool Success,string Message)> AddToCartAsync(string userId,int productId,int quantity=1)
        {
            try
            {
                var product = await _context.Products.FindAsync(productId);

                if (product == null)
                    return (false, "Product not found");
                if (product.Stock < quantity)
                    return (false, "Insufficient Stock");

                var existing = await _context.CartItems.FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);

                if(existing != null)
                {
                    existing.Quantity += quantity;
                }
                else
                {
                    _context.CartItems.Add(new CartItem
                    {
                        UserId = userId,
                        ProductId = productId,
                        Quantity = quantity,
                        AddedAt = DateTime.UtcNow
                    });
                }

                await _context.SaveChangesAsync();
                return (true, "Added to cart successfully");
            }catch(Exception ex)
            {
                return (false, $"Error adding to cart: {ex.Message}");
            }

        }

        public async Task<(bool Success, string Message)> UpdateQuantityAsync(
            int cartItemId, int quantity)
        {
            try
            {
                var item = await _context.CartItems.FindAsync(cartItemId);
                if (item == null)
                    return (false, "Cart item not found.");

                if (quantity <= 0)
                {
                    _context.CartItems.Remove(item);
                }
                else
                {
                    item.Quantity = quantity;
                }

                await _context.SaveChangesAsync();
                return (true, "Cart updated.");
            }
            catch (Exception ex)
            {
                return (false, $"Error updating cart: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> RemoveFromCartAsync(
            int cartItemId)
        {
            try
            {
                var item = await _context.CartItems.FindAsync(cartItemId);
                if (item == null)
                    return (false, "Cart item not found.");

                _context.CartItems.Remove(item);
                await _context.SaveChangesAsync();
                return (true, "Removed from cart.");
            }
            catch (Exception ex)
            {
                return (false, $"Error removing from cart: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> ClearCartAsync(
            string userId)
        {
            try
            {
                var items = await _context.CartItems
                    .Where(c => c.UserId == userId)
                    .ToListAsync();

                _context.CartItems.RemoveRange(items);
                await _context.SaveChangesAsync();
                return (true, "Cart cleared.");
            }
            catch (Exception ex)
            {
                return (false, $"Error clearing cart: {ex.Message}");
            }
        }
    }
}