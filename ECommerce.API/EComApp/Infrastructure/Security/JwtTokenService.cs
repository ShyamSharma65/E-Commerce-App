using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ECommerceAPI.Domain.Interfaces;
using ECommerceAPI.Models;
using Microsoft.IdentityModel.Tokens;

namespace ECommerceAPI.Infrastructure.Security
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly IConfiguration _configuration;

        public JwtTokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(Customer customer)
        {
            var jwtKey = _configuration["Jwt:Key"] ?? "ECommerceApp_Super_Secret_Key_2026_!@#$%^&*()";
            var jwtIssuer = _configuration["Jwt:Issuer"] ?? "ECommerceApi";
            var jwtAudience = _configuration["Jwt:Audience"] ?? "ECommerceClient";

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, customer.Id.ToString()),
                new Claim(ClaimTypes.NameIdentifier, customer.Id.ToString()),
                new Claim(ClaimTypes.Name, customer.Email),
                new Claim(ClaimTypes.Email, customer.Email),
                new Claim(ClaimTypes.Role, string.IsNullOrWhiteSpace(customer.Role) ? "Customer" : customer.Role),
                new Claim("customerId", customer.Id.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
