using ECommerce.Data;
using ECommerce.Models;
using ECommerce.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _factory;

        public CategoryService(IDbContextFactory<ApplicationDbContext> factory)
        {
            _factory = factory;
        }

        public async Task<List<Category>> GetAllAsync()
        {
            await using var context = await _factory.CreateDbContextAsync();
            return await context.Categories
                .Include(c => c.Products)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<Category?> GetByIdAsync(int id)
        {
            await using var context = await _factory.CreateDbContextAsync();
            return await context.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<bool> ExistsAsync(string name)
        {
            await using var context = await _factory.CreateDbContextAsync();
            return await context.Categories
                .AnyAsync(c => c.Name.ToLower() == name.ToLower());
        }

        public async Task<(bool Success, string Message)> CreateAsync(Category category)
        {
            try
            {
                if (await ExistsAsync(category.Name))
                    return (false, "Category with this name already exists.");

                await using var context = await _factory.CreateDbContextAsync();
                category.CreatedAt = DateTime.UtcNow;
                context.Categories.Add(category);
                await context.SaveChangesAsync();
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
                await using var context = await _factory.CreateDbContextAsync();
                var existing = await context.Categories.FindAsync(category.Id);
                if (existing == null)
                    return (false, "Category not found.");

                existing.Name = category.Name;
                existing.Description = category.Description;
                existing.ImageUrl = category.ImageUrl;
                existing.IsActive = category.IsActive;

                await context.SaveChangesAsync();
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
                await using var context = await _factory.CreateDbContextAsync();
                var category = await context.Categories
                    .Include(c => c.Products)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (category == null)
                    return (false, "Category not found.");

                if (category.Products.Any())
                    return (false, "Cannot delete category with existing products.");

                context.Categories.Remove(category);
                await context.SaveChangesAsync();
                return (true, "Category deleted successfully.");
            }
            catch (Exception ex)
            {
                return (false, $"Error deleting category: {ex.Message}");
            }
        }
    }
}