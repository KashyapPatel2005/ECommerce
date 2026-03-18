
using ECommerce.Models;

namespace ECommerce.Services.Interfaces
{
    public interface IAddressService
    {
        Task<Address?> SaveAddressAsync(Address address);
    }
}