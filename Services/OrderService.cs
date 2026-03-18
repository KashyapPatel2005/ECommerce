using ECommerce.Data;
using ECommerce.Models;
using ECommerce.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Services
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;

        public OrderService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Order>> GetAllAsync()
        {
          
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<List<Order>> GetByUserAsync(string userId)
        {
         
            return await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<Order?> GetByIdAsync(int id)
        {
           
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<int> GetTotalOrdersCountAsync()
        {
         
            return await _context.Orders.CountAsync();
        }

        public async Task<int> GetPendingOrdersCountAsync()
        {
           
            return await _context.Orders
                .CountAsync(o => o.Status == OrderStatus.Pending);
        }

        public async Task<decimal> GetTotalRevenueAsync()
        {
            return await _context.Orders
                .Where(o => o.Status != OrderStatus.Cancelled)
                .SumAsync(o => o.TotalAmount);
        }

        public async Task<decimal> GetMonthlyRevenueAsync()
        {
            var start = new DateTime(
                DateTime.Now.Year,
                DateTime.Now.Month, 1);
            return await _context.Orders
                .Where(o => o.OrderDate >= start
                         && o.Status != OrderStatus.Cancelled)
                .SumAsync(o => o.TotalAmount);
        }

        public async Task<List<Order>> GetRecentAsync(int count)
        {
 
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .OrderByDescending(o => o.OrderDate)
                .Take(count)
                .ToListAsync();
        }

        public async Task<Dictionary<string, decimal>>
            GetMonthlyRevenueChartAsync()
        {
            var sixMonthsAgo = DateTime.Now.AddMonths(-6);
            var orders = await _context.Orders
                .Where(o => o.OrderDate >= sixMonthsAgo
                         && o.Status != OrderStatus.Cancelled)
                .ToListAsync();

            return orders
                .GroupBy(o => o.OrderDate.ToString("MMM yyyy"))
                .ToDictionary(
                    g => g.Key,
                    g => g.Sum(o => o.TotalAmount));
        }

        public async Task<(bool Success, string Message)> CreateAsync(
            Order order)
        {
            try
            {
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
                return (true, "Order placed successfully.");
            }
            catch (Exception ex)
            {
                return (false, $"Error placing order: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> UpdateStatusAsync(
            int orderId, OrderStatus status)
        {
            try
            {
             
                var order = await _context.Orders.FindAsync(orderId);
                if (order == null)
                    return (false, "Order not found.");

                order.Status = status;
                await _context.SaveChangesAsync();
                return (true, "Order status updated.");
            }
            catch (Exception ex)
            {
                return (false, $"Error updating order: {ex.Message}");
            }
        }
    }
}