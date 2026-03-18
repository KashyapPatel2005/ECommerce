using ECommerce.Data;
using ECommerce.Models;
using ECommerce.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _context;

        public CategoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Category>> GetAllAsync()
        {
            return await _context.Categories
                .Include(c => c.Products)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<Category?> GetByIdAsync(int id)
        {
            return await _context.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<bool> ExistsAsync(string name)
        {
            return await _context.Categories
                .AnyAsync(c => c.Name.ToLower() == name.ToLower());
        }

        public async Task<(bool Success, string Message)> CreateAsync(Category category)
        {
            try
            {
                if (await ExistsAsync(category.Name))
                    return (false, "Category with this name already exists.");

                category.CreatedAt = DateTime.UtcNow;
                _context.Categories.Add(category);
                await _context.SaveChangesAsync();
                return (true, "Category created successfully.");
            }
            catch (Exception ex)
            {
                return (false, $"Error creating category: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> UpdateAsync(Category category)
        {
            try
            {
                var existing = await _context.Categories.FindAsync(category.Id);
                if (existing == null)
                    return (false, "Category not found.");

                existing.Name = category.Name;
                existing.Description = category.Description;
                existing.ImageUrl = category.ImageUrl;
                existing.IsActive = category.IsActive;

                await _context.SaveChangesAsync();
                return (true, "Category updated successfully.");
            }
            catch (Exception ex)
            {
                return (false, $"Error updating category: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> DeleteAsync(int id)
        {
            try
            {
                var category = await _context.Categories
                    .Include(c => c.Products)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (category == null)
                    return (false, "Category not found.");

                if (category.Products.Any())
                    return (false, "Cannot delete category with existing products.");

                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
                return (true, "Category deleted successfully.");
            }
            catch (Exception ex)
            {
                return (false, $"Error deleting category: {ex.Message}");
            }
        }
    }
}