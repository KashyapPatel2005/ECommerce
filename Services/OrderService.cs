using ECommerce.Data;
using ECommerce.Models;
using ECommerce.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Services
{
    public class OrderService : IOrderService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _factory;

        public OrderService(IDbContextFactory<ApplicationDbContext> factory)
        {
            _factory = factory;
        }

        public async Task<List<Order>> GetAllAsync()
        {
            await using var context = await _factory.CreateDbContextAsync();
            return await context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<List<Order>> GetByUserAsync(string userId)
        {
            await using var context = await _factory.CreateDbContextAsync();
            return await context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<Order?> GetByIdAsync(int id)
        {
            await using var context = await _factory.CreateDbContextAsync();
            return await context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<int> GetTotalOrdersCountAsync()
        {
            await using var context = await _factory.CreateDbContextAsync();
            return await context.Orders.CountAsync();
        }

        public async Task<int> GetPendingOrdersCountAsync()
        {
            await using var context = await _factory.CreateDbContextAsync();
            return await context.Orders
                .CountAsync(o => o.Status == OrderStatus.Pending);
        }

        public async Task<decimal> GetTotalRevenueAsync()
        {
            await using var context = await _factory.CreateDbContextAsync();
            return await context.Orders
                .Where(o => o.Status != OrderStatus.Cancelled)
                .SumAsync(o => o.TotalAmount);
        }

        public async Task<decimal> GetMonthlyRevenueAsync()
        {
            await using var context = await _factory.CreateDbContextAsync();
            var start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            return await context.Orders
                .Where(o => o.OrderDate >= start && o.Status != OrderStatus.Cancelled)
                .SumAsync(o => o.TotalAmount);
        }

        public async Task<List<Order>> GetRecentAsync(int count)
        {
            await using var context = await _factory.CreateDbContextAsync();
            return await context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .OrderByDescending(o => o.OrderDate)
                .Take(count)
                .ToListAsync();
        }

        public async Task<Dictionary<string, decimal>> GetMonthlyRevenueChartAsync()
        {
            await using var context = await _factory.CreateDbContextAsync();
            var sixMonthsAgo = DateTime.Now.AddMonths(-6);
            var orders = await context.Orders
                .Where(o => o.OrderDate >= sixMonthsAgo && o.Status != OrderStatus.Cancelled)
                .ToListAsync();

            return orders
                .GroupBy(o => o.OrderDate.ToString("MMM yyyy"))
                .ToDictionary(
                    g => g.Key,
                    g => g.Sum(o => o.TotalAmount));
        }

        public async Task<(bool Success, string Message)> CreateAsync(Order order)
        {
            try
            {
                await using var context = await _factory.CreateDbContextAsync();
                context.Orders.Add(order);
                await context.SaveChangesAsync();
                return (true, "Order placed successfully.");
            }
            catch (Exception ex)
            {
                return (false, $"Error placing order: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> UpdateStatusAsync(int orderId, OrderStatus status)
        {
            try
            {
                await using var context = await _factory.CreateDbContextAsync();
                var order = await context.Orders.FindAsync(orderId);
                if (order == null)
                    return (false, "Order not found.");

                order.Status = status;
                await context.SaveChangesAsync();
                return (true, "Order status updated.");
            }
            catch (Exception ex)
            {
                return (false, $"Error updating order: {ex.Message}");
            }
        }
    }
}