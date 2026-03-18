using ECommerce.Models;

namespace ECommerce.Services.Interfaces
{
    public interface IProductService
    {
        Task<List<Product>> GetAllAsync();
        Task<Product?> GetByIdAsync(int id);
        Task<List<Product>> GetByCategoryAsync(int categoryId);
        Task<List<Product>> SearchAsync(string query);
        Task<List<Product>> GetLowStockAsync(int threshold = 10);
        Task<(bool Success, string Message)> CreateAsync(Product product);
        Task<(bool Success, string Message)> UpdateAsync(Product product);
        Task<(bool Success, string Message)> DeleteAsync(int id);
        Task<int> GetTotalCountAsync();

        Task<List<Category>> GetCategoriesAsync();
    }
}