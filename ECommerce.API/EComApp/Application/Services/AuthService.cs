using ECommerceAPI.AppDbContext;
using ECommerceAPI.Application.DTOs;
using ECommerceAPI.Domain.Interfaces;
using ECommerceAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI.Application.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request);
        Task<AdminRegistrationResponseDto> RegisterAdminAsync(RegisterRequestDto request);
        Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
    }

    public class AuthService : IAuthService
    {
        private readonly EComDbContext _context;
        private readonly IJwtTokenService _jwtTokenService;

        public AuthService(EComDbContext context, IJwtTokenService jwtTokenService)
        {
            _context = context;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request)
        {
            var customer = await CreateAccountAsync(request, "Customer");

            var token = _jwtTokenService.GenerateToken(customer);

            return new AuthResponseDto
            {
                Id = customer.Id,
                Email = customer.Email,
                FullName = $"{customer.FirstName} {customer.LastName}".Trim(),
                Role = customer.Role,
                Token = token,
                Message = "Registration successful."
            };
        }

        public async Task<AdminRegistrationResponseDto> RegisterAdminAsync(RegisterRequestDto request)
        {
            var admin = await CreateAccountAsync(request, "Admin");

            return new AdminRegistrationResponseDto
            {
                Id = admin.Id,
                Email = admin.Email,
                FullName = $"{admin.FirstName} {admin.LastName}".Trim(),
                Role = admin.Role,
                Message = "Admin account created successfully."
            };
        }

        private async Task<Customer> CreateAccountAsync(RegisterRequestDto request, string role)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                throw new InvalidOperationException("Email and password are required.");

            if (await _context.Customers.AnyAsync(customer => customer.Email == request.Email))
                throw new InvalidOperationException("User already registered with this email.");

            var customer = new Customer
            {
                FirstName = request.FirstName,
                MiddleName = request.MiddleName,
                LastName = request.LastName,
                Email = request.Email,
                PhoneNumber = request.Phone,
                DateOfBirth = request.DateOfBirth,
                Password = request.Password,
                Role = role,
                IsActive = true
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
            return customer;
        }

        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Email == request.Email);
            if (customer == null || customer.Password != request.Password)
                throw new InvalidOperationException("Invalid email or password.");

            var token = _jwtTokenService.GenerateToken(customer);

            return new AuthResponseDto
            {
                Id = customer.Id,
                Email = customer.Email,
                FullName = $"{customer.FirstName} {customer.LastName}".Trim(),
                Role = string.IsNullOrWhiteSpace(customer.Role) ? "Customer" : customer.Role,
                Token = token,
                Message = "Login successful."
            };
        }
    }
}
