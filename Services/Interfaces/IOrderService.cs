using ECommerce.Models;

namespace ECommerce.Services.Interfaces
{
    public interface IOrderService
    {
        Task<List<Order>> GetAllAsync();
        Task<List<Order>> GetByUserAsync(string userId);
        Task<Order?> GetByIdAsync(int id);
        Task<int> GetTotalOrdersCountAsync();
        Task<int> GetPendingOrdersCountAsync();
        Task<decimal> GetTotalRevenueAsync();
        Task<decimal> GetMonthlyRevenueAsync();
        Task<List<Order>> GetRecentAsync(int count);
        Task<Dictionary<string, decimal>> GetMonthlyRevenueChartAsync();
        Task<(bool Success, string Message)> CreateAsync(Order order);
        Task<(bool Success, string Message)> UpdateStatusAsync(
            int orderId, OrderStatus status);
    }
}