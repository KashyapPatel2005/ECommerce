using ECommerce.Models;

namespace ECommerce.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<List<Category>> GetAllAsync();
        Task<Category?> GetByIdAsync(int id);
        Task<(bool Success, string Message)> CreateAsync(Category category);
        Task<(bool Success, string Message)> UpdateAsync(Category category);
        Task<(bool Success, string Message)> DeleteAsync(int id);
        Task<bool> ExistsAsync(string name);
    }
}