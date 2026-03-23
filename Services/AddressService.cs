using ECommerce.Data;
using ECommerce.Models;
using ECommerce.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Services
{
    public class AddressService : IAddressService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _factory;

        public AddressService(IDbContextFactory<ApplicationDbContext> factory)
        {
            _factory = factory;
        }

        public async Task<Address?> SaveAddressAsync(Address address)
        {
            try
            {
                await using var context = await _factory.CreateDbContextAsync();
                context.Addresses.Add(address);
                await context.SaveChangesAsync();
                return address;
            }
            catch
            {
                return null;
            }
        }
    }
}