using ECommerce.Data;

namespace ECommerce.Services.Interfaces
{
    public interface IUserService
    {
        Task<List<ApplicationUser>> GetAllAsync();
        Task<ApplicationUser?> GetByIdAsync(string id);
        Task<(bool Success, string Message)> UpdateStatusAsync(string id, bool isActive);
        Task<(bool Success, string Message)> ChangeRoleAsync(string id, string newRole);
        Task<int> GetTotalUsersCountAsync();
        Task<int> GetNewUsersThisMonthAsync();
        Task<string> GetUserRoleAsync(string userId);
    }
}