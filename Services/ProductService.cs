using ECommerce.Data;
using ECommerce.Models;
using ECommerce.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Services
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;

        public ProductService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Product>> GetAllAsync()
        {
         
            return await _context.Products.ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
        
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Include(p => p.Reviews)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<Product>> GetByCategoryAsync(int categoryId)
        {
          
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Where(p => p.CategoryId == categoryId && p.IsActive)
                .ToListAsync();
        }

        public async Task<List<Product>> SearchAsync(string query)
        {
          
            return await _context.Products
                .Include(p => p.Category)
                .Where(p => p.Name.Contains(query) ||
                            (p.Description != null &&
                             p.Description.Contains(query)))
                .ToListAsync();
        }

        public async Task<List<Product>> GetLowStockAsync(int threshold = 10)
        {
    
            return await _context.Products
                .Include(p => p.Category)
                .Where(p => p.Stock <= threshold)
                .OrderBy(p => p.Stock)
                .ToListAsync();
        }



        public async Task<int> GetTotalCountAsync()
        {
          
            return await _context.Products.CountAsync();
        }

        public async Task<(bool Success, string Message)> CreateAsync(Product product)
        {
           
            try
            {
                product.CreatedAt = DateTime.UtcNow;
                product.UpdatedAt = DateTime.UtcNow;
                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                return (true, "Product created successfully.");
            }
            catch (Exception ex)
            {
                return (false, $"Error creating product: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> UpdateAsync(Product product)
        {
     
            try
            {
                var existing = await _context.Products.FindAsync(product.Id);
                if (existing == null)
                    return (false, "Product not found.");

                existing.Name = product.Name;
                existing.Description = product.Description;
                existing.Price = product.Price;
                existing.DiscountPrice = product.DiscountPrice;
                existing.Stock = product.Stock;
                existing.CategoryId = product.CategoryId;
                existing.MainImageUrl = product.MainImageUrl;
                existing.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return (true, "Product updated successfully.");
            }
            catch (Exception ex)
            {
                return (false, $"Error updating product: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> DeleteAsync(int id)
        {
           
            try
            {
                var product = await _context.Products.FindAsync(id);
                if (product == null)
                    return (false, "Product not found.");

                _context.Products.Remove(product); // ← Hard delete
                await _context.SaveChangesAsync();
                return (true, "Product deleted successfully.");
            }
            catch (Exception ex)
            {
                return (false, $"Error deleting product: {ex.Message}");
            }
        }

        public async Task<List<Category>> GetCategoriesAsync()
        {
         
            return await _context.Categories
                .Where(c => c.IsActive)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }
    }
}

