using ECommerceAPI.Models;

namespace ECommerceAPI.Domain.Interfaces
{
    public interface IJwtTokenService
    {
        string GenerateToken(Customer customer);
    }
}
